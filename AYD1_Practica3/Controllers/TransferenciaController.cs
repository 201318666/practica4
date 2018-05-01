using AYD1_Practica3.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace AYD1_Practica3.Controllers
{
    public class TransferenciaController : Controller
    {
        // GET: /Transferencia/CrearTransferencia
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Transferencia/CrearTransferencia
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(TransferenciaModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine(model.AccountNumber);
                return View(model);
            }
            string cuenta_pm = model.AccountNumber;
            string usuario = User.Identity.Name;
            if (ExisteDestino(cuenta_pm) && HayFondos(usuario, Convert.ToDouble(model.Balance)))
            {
                SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\C45ASP4311F\\source\\repos\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");

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
                    CommandText = "select Balance from AspNetUsers where AccountNumber='" + model.AccountNumber + "';",
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

                    double monto = Convert.ToDouble(model.Balance);
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
                        CommandText = "UPDATE AspNetUsers SET Balance='" + monto_destino + "' WHERE AccountNumber='" + model.AccountNumber + "';",
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
                return View("EndTransferencia");
            }
            else
            {
                ModelState.AddModelError("", "Numero de cuenta no existe.");
                return View(model);
            }
        }


        public bool ExisteDestino(string cuenta)
        {
            SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\C45ASP4311F\\source\\repos\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");
            //Cuenta destino
            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "select count(AccountNumber) from AspNetUsers where AccountNumber='" + cuenta + "';",
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

        public bool HayFondos(string usuario, double monto)
        {
            SqlConnection sqlCon = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\C45ASP4311F\\source\\repos\\AYD1_Practica3\\AYD1_Practica3\\App_Data\\aspnet-AYD1_Practica3-20180410094646.mdf;Initial Catalog=aspnet-AYD1_Practica3-20180410094646;Integrated Security=True");
            //Monto de usuario logueado
            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "select Balance from AspNetUsers where UserName='" + usuario + "';",
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

    }
}
