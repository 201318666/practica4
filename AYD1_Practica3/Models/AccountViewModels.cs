using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AYD1_Practica3.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "¿Recordar este explorador?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class CheckBalance
    {
        [Required]
        [Display(Name = "Saldo")]
        [DataType(DataType.Text)]
        public string Email { get; set; }
    }


    public class TransferViewModel
    {
        [Required]
        [Display(Name = "Número de cuenta destino")]
        [DataType(DataType.Text)]
        public string DestinationAccountNumber { get; set; }

        [Required]
        [Display(Name = "Monto a transferir")]
        [DataType(DataType.Text)]
        public string Amount { get; set; }

    }
    

    public class CreditViewModel
    {
        [Required]
        [Display(Name = "Número de cuenta del usuario a acreditar")]
        [DataType(DataType.Text)]
        public string DestinationAccountNumber { get; set; }

        [Required]
        [Display(Name = "Monto a acreditar")]
        [DataType(DataType.Text)]
        public string Amount { get; set; }

        [Required]
        [Display(Name = "Descripcion del credito")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

    }

    public class DebitViewModel
    {
        [Required]
        [Display(Name = "Número de cuenta del usuario a debitar")]
        [DataType(DataType.Text)]
        public string DestinationAccountNumber { get; set; }

        [Required]
        [Display(Name = "Monto a debitar")]
        [DataType(DataType.Text)]
        public string Amount { get; set; }

        [Required]
        [Display(Name = "Descripcion del debito")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

    }


    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Número de cuenta")]
        [DataType(DataType.Text)]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        [DataType(DataType.Text)]
        public string User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "¿Recordar cuenta?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Nombre completo")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        [DataType(DataType.Text)]
        public string User { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }
}
