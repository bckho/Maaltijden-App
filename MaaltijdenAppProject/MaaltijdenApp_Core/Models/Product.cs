using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_Core.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public bool IsExample { get; set; }

        public string Description { get; set; } = null!;

        public bool ContainsAlcohol { get; set; }

        public byte[]? Image { get; set; } = null;

        public ICollection<MealPackage>? MealPackages { get; set; }
    }
}
