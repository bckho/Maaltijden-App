using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_Core.Models
{
    public class Student
    {
        [Key]
        public Guid Id { get; set; }

        public string LastName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public ICollection<MealPackage>? ReservedMealPackages { get; set; }

        public int NoShowCount { get; set; }

        public int GetAge()
        {
            var today = DateTime.Now;

            var age = BirthDate.Year - today.Year;

            if (BirthDate > today.AddYears(-age)) age--;

            return age;
        }

        public bool IsEighteenPlusAtDate(DateTime date)
        {
            var age = BirthDate.Year - date.Year;

            if (BirthDate > date.AddYears(-age)) age--;

            if (age >= 18)
            {
                return true;
            }
            return false;
        }

        public bool AgeIsValid()
        {
            var age = GetAge();

            if (age >= 16) return true;

            return false;
        }
    }
}
