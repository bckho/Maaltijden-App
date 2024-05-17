using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaltijdenApp_Core.Models
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeNumber { get; set; }

        public string LastName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string Email { get; set; } = null!;

        public Guid CanteenId { get; set; }

        public Canteen? Canteen { get; set; }
    }
}
