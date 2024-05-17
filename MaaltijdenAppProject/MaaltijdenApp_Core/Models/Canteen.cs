using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_Core.Models
{
    public class Canteen
    {
        [Key]
        public Guid Id { get; set; }

        public CityEnum City { get; set; }

        public string Location { get; set; } = null!;

        public bool CanServeHotDinnerMeals { get; set; } = false;

        public ICollection<Employee> Employees { get; set; } = null!;
    }
}
