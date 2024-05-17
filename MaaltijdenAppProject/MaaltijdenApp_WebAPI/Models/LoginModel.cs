using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_WebAPI.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        public string? EmailAddress { get; set; }

        [Required(ErrorMessage ="Password is required.")]
        public string? Password { get; set; }
    }
}
