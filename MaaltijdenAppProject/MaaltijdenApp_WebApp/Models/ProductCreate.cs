using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_WebApp.Models
{
    public class ProductCreate
    {
        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Korte omschrijving is verplicht.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Alcohol indicatie is verplicht.")]
        public bool ContainsAlcohol { get; set; }

        public IFormFile? Image { get; set; }
    }
}
