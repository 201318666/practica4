using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AYD1_Practica3.Models;
using System.Data.SqlClient;
using System.Data;

namespace AYD1_Practica3.Controllers
{
    [Authorize]
    public class AccountController : ApplicationBaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        //
        // GET: /Account/CheckBalance
        [AllowAnonymous]
        public ActionResult CheckBalance(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Javier\\Desktop\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");
            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "select Balance from AspNetUsers where UserName='" + User.Identity.Name + "';",
                Connection = sqlCon
            };
            string consulta = "";
            try
            {
                sqlCon.Open();
                consulta = sqlCmd.ExecuteScalar().ToString();
            }
            catch
            {
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                    sqlCon.Close();
            }
            ViewBag.Message = consulta;
            return View();
        }

        //
        // GET: /Account/Transfer
        [AllowAnonymous]
        public ActionResult Transfer(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Transfer
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Transfer(TransferViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                Console.WriteLine(model.DestinationAccountNumber);
                return View(model);
            }
            if (ExisteDestino(model.DestinationAccountNumber) && HayFondosUsuarioLogueado(Convert.ToDouble(model.Amount)))
            {
                SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Javier\\Desktop\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");

                //Monto de usuario logueado
                SqlCommand sqlCmd = new SqlCommand
                {
                    CommandText = "select Balance from AspNetUsers where UserName='" + User.Identity.Name + "';",
                    Connection = sqlCon
                };

                //Cuenta de usuario logueado
                SqlCommand sqlCmd2 = new SqlCommand
                {
                    CommandText = "select AccountNumber from AspNetUsers where UserName='" + User.Identity.Name + "';",
                    Connection = sqlCon
                };

                //Monto de cuenta destino
                SqlCommand sqlCmd3 = new SqlCommand
                {
                    CommandText = "select Balance from AspNetUsers where AccountNumber='" + model.DestinationAccountNumber + "';",
                    Connection = sqlCon
                };

                string consultadestino = "0";
                string consultaCuenta = "0";
                string consultaMonto = "0";

                //este mensaje nunca deberia de imprimirse
                ViewBag.Message = String.Format("Aun no he realizado la transferencia\\n{0}", DateTime.Now.ToString());
                //lo puse para probar porque la estaba cagando en algo
                try
                {
                    sqlCon.Open();
                    consultaCuenta = sqlCmd2.ExecuteScalar().ToString();
                    consultaMonto = sqlCmd.ExecuteScalar().ToString();
                    consultadestino = sqlCmd3.ExecuteScalar().ToString();

                    double monto = Convert.ToDouble(model.Amount);
                    string monto_usuario_logueado = Convert.ToString(Convert.ToDouble(consultaMonto) - monto);
                    string monto_destino = Convert.ToString(Convert.ToDouble(consultadestino) + monto);

                    //Actualiza monto actual de usuario logueado
                    SqlCommand usuario_origen = new SqlCommand
                    {
                        CommandText = "UPDATE AspNetUsers SET Balance='" + monto_usuario_logueado + "' WHERE UserName='" + User.Identity.Name + "';",
                        Connection = sqlCon
                    };

                    usuario_origen.ExecuteNonQuery();

                    //Actualiza monto actual de usuario destino
                    SqlCommand usuario_destino = new SqlCommand
                    {
                        CommandText = "UPDATE AspNetUsers SET Balance='" + monto_destino + "' WHERE AccountNumber='" + model.DestinationAccountNumber + "';",
                        Connection = sqlCon
                    };
                    usuario_destino.ExecuteNonQuery();

                    ViewBag.Message = String.Format("Transferencia realizada con exito\\n Fecha y hora: {0}", DateTime.Now.ToString());
                }
                catch
                {
                    ViewBag.Message = String.Format("La estoy cagando\\n{0}", DateTime.Now.ToString());
                }
                finally
                {
                    if (sqlCon.State == ConnectionState.Open)
                    {
                        sqlCon.Close();
                    }
                }
                return RedirectToAction("CheckBalance", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Numero de cuenta no existe.");
                return View(model);
            }

        }


        private bool ExisteDestino(string DestinationAccountNumber)
        {
            SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Javier\\Desktop\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");
            //Cuenta destino
            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "select count(AccountNumber) from AspNetUsers where AccountNumber='" + DestinationAccountNumber + "';",
                Connection = sqlCon
            };
            string consulta = "0";
            try
            {
                sqlCon.Open();
                consulta = sqlCmd.ExecuteScalar().ToString();
            }
            catch
            {
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
            if (consulta.Equals("1"))
                return true;
            return false;
        }

        private bool HayFondosUsuarioLogueado(double monto)
        {
            SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Javier\\Desktop\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");
            //Monto de usuario logueado
            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "select Balance from AspNetUsers where UserName='" + User.Identity.Name + "';",
                Connection = sqlCon
            };
            string consultaMonto = "0";
            try
            {
                sqlCon.Open();
                consultaMonto = sqlCmd.ExecuteScalar().ToString();
            }
            catch
            {
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
            double lo_que_hay = Convert.ToDouble(consultaMonto);
            if (lo_que_hay >= monto)
                return true;
            return false;
        }

        private bool HayFondos(string DestinationAccountNumber, double monto)
        {
            SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Javier\\Desktop\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");
            //Monto de usuario logueado
            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "select Balance from AspNetUsers where AccountNumber='" + DestinationAccountNumber + "';",
                Connection = sqlCon
            };
            string consultaMonto = "0";
            try
            {
                sqlCon.Open();
                consultaMonto = sqlCmd.ExecuteScalar().ToString();
            }
            catch
            {
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
            double lo_que_hay = Convert.ToDouble(consultaMonto);
            if (lo_que_hay >= monto)
                return true;
            return false;
        }

        //
        // GET: /Account/Credit
        [AllowAnonymous]
        public ActionResult Credit(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // GET: /Account/IndexUser
        [AllowAnonymous]
        public ActionResult IndexUser(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        //
        // POST: /Account/Credit
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Credit(CreditViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine(model.DestinationAccountNumber);
                return View(model);
            }
            if (ExisteDestino(model.DestinationAccountNumber))
            {
                SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Javier\\Desktop\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");

                //Monto de cuenta destino
                SqlCommand sqlCmd3 = new SqlCommand
                {
                    CommandText = "select Balance from AspNetUsers where AccountNumber='" + model.DestinationAccountNumber + "';",
                    Connection = sqlCon
                };

                string consultadestino = "0";

                //este mensaje nunca deberia de imprimirse
                ViewBag.Message = String.Format("Aun no he realizado la transferencia\\n{0}", DateTime.Now.ToString());
                //lo puse para probar porque la estaba cagando en algo
                try
                {
                    sqlCon.Open();
                    consultadestino = sqlCmd3.ExecuteScalar().ToString();

                    double monto = Convert.ToDouble(model.Amount);
                    string monto_destino = Convert.ToString(Convert.ToDouble(consultadestino) + monto);

                    //Actualiza monto actual de usuario destino
                    SqlCommand usuario_destino = new SqlCommand
                    {
                        CommandText = "UPDATE AspNetUsers SET Balance='" + monto_destino + "' WHERE AccountNumber='" + model.DestinationAccountNumber + "';",
                        Connection = sqlCon
                    };
                    usuario_destino.ExecuteNonQuery();

                    ViewBag.Message = String.Format("Transferencia realizada con exito\\n Fecha y hora: {0}", DateTime.Now.ToString());
                }
                catch
                {
                    ViewBag.Message = String.Format("La estoy cagando\\n{0}", DateTime.Now.ToString());
                }
                finally
                {
                    if (sqlCon.State == ConnectionState.Open)
                    {
                        sqlCon.Close();
                    }
                }
                return RedirectToAction("CheckBalance", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Numero de cuenta no existe.");
                return View(model);
            }

        }


        //
        // GET: /Account/Debit
        [AllowAnonymous]
        public ActionResult Debit(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        //
        // POST: /Account/Debit
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Debit(DebitViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine(model.DestinationAccountNumber);
                return View(model);
            }
            if (ExisteDestino(model.DestinationAccountNumber) && HayFondos(model.DestinationAccountNumber, Convert.ToDouble(model.Amount)))
            {
                SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Javier\\Desktop\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");

                //Monto de cuenta destino
                SqlCommand sqlCmd3 = new SqlCommand
                {
                    CommandText = "select Balance from AspNetUsers where AccountNumber='" + model.DestinationAccountNumber + "';",
                    Connection = sqlCon
                };

                string consultadestino = "0";

                //este mensaje nunca deberia de imprimirse
                ViewBag.Message = String.Format("Aun no he realizado la transferencia\\n{0}", DateTime.Now.ToString());
                //lo puse para probar porque la estaba cagando en algo
                try
                {
                    sqlCon.Open();
                    consultadestino = sqlCmd3.ExecuteScalar().ToString();

                    double monto = Convert.ToDouble(model.Amount);
                    string monto_destino = Convert.ToString(Convert.ToDouble(consultadestino) - monto);

                    //Actualiza monto actual de usuario destino
                    SqlCommand usuario_destino = new SqlCommand
                    {
                        CommandText = "UPDATE AspNetUsers SET Balance='" + monto_destino + "' WHERE AccountNumber='" + model.DestinationAccountNumber + "';",
                        Connection = sqlCon
                    };
                    usuario_destino.ExecuteNonQuery();

                    ViewBag.Message = String.Format("Transferencia realizada con exito\\n Fecha y hora: {0}", DateTime.Now.ToString());
                }
                catch
                {
                    ViewBag.Message = String.Format("La estoy cagando\\n{0}", DateTime.Now.ToString());
                }
                finally
                {
                    if (sqlCon.State == ConnectionState.Open)
                    {
                        sqlCon.Close();
                    }
                }
                return RedirectToAction("CheckBalance", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Numero de cuenta no existe.");
                return View(model);
            }
        }



        //
        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Javier\\Desktop\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");
            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "select count(AccountNumber) from AspNetUsers where AccountNumber='" + model.AccountNumber + "';",
                Connection = sqlCon
            };
            string consulta = "0";
            try
            {
                sqlCon.Open();
                consulta = sqlCmd.ExecuteScalar().ToString();
            }
            catch
            {
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                    sqlCon.Close();
            }

            if (consulta.Equals("1"))
            {
                // No cuenta los errores de inicio de sesión para el bloqueo de la cuenta
                // Para permitir que los errores de contraseña desencadenen el bloqueo de la cuenta, cambie a shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.User, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        ViewBag.Message = String.Format("Bienvenido {0}!.\\nNumero de cuenta: {1} \\nFecha y hora: {2}", model.User, model.AccountNumber, DateTime.Now.ToString());
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Intento de inicio de sesión no válido.");
                        return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Numero de cuenta no concuerda con ninguna registrada.");
                return View(model);
            }

        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Requerir que el usuario haya iniciado sesión con nombre de usuario y contraseña o inicio de sesión externo
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // El código siguiente protege de los ataques por fuerza bruta a los códigos de dos factores. 
            // Si un usuario introduce códigos incorrectos durante un intervalo especificado de tiempo, la cuenta del usuario 
            // se bloqueará durante un período de tiempo especificado. 
            // Puede configurar el bloqueo de la cuenta en IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Código no válido.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string cuenta = RandomString();
                var user = new ApplicationUser { UserName = model.User, Email = model.Email, AccountNumber = cuenta, UserNameComplete = model.Username, Balance = "0.0" };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
                    // Enviar correo electrónico con este vínculo
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                    ViewBag.Message = String.Format("Usuario {0} registrado con exito!.\\n Numero de cuenta: {1} \\nRecuerde apuntar el numero.\\n Fecha y hora: {2}", model.User, cuenta, DateTime.Now.ToString());
                    return View("Login");
                }
                AddErrors(result);
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        private static Random random = new Random();
        public static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 5)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ForgotPasswordConfirmation");
                }

                // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Restablecer contraseña", "Para restablecer la contraseña, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Solicitar redireccionamiento al proveedor de inicio de sesión externo
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generar el token y enviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Si el usuario ya tiene un inicio de sesión, iniciar sesión del usuario con este proveedor de inicio de sesión externo
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Si el usuario no tiene ninguna cuenta, solicitar que cree una
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtener datos del usuario del proveedor de inicio de sesión externo
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}