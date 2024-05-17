using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_WebApp.Models
{
    public class AccountLogin
    {
        [Required(ErrorMessage = "Emailadres is verplicht.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        public string? Password { get; set; }
    }
}
