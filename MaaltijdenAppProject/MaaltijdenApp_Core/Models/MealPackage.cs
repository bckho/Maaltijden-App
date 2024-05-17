using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_Core.Models
{
    public class MealPackage
    {
        [Key]
        public Guid Id { get; set; }

        public ICollection<Product> ProductsIndicator { get; set; } = null!;

        public CityEnum City { get; set; }

        public Guid CanteenId { get; set; }

        public Canteen? Canteen { get; set; }

        public DateTime StartPickupDateTime { get; set; }

        public DateTime EndPickupDateTime { get; set; }

        public decimal Price { get; set; }

        public MealTypeEnum MealType { get; set; }

        public bool IsHotMeal { get; set; }

        public bool IsEighteenPlusOnly { get; set; }

        public Guid? ReservedByStudentId { get; set; }

        public Student? ReservedByStudent { get; set; }

        public bool IsNoShow { get; set; } = false;

        /// <summary>
        /// Check if Pickup and EndPickup datetimes are valid.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool PickUpDateIsValid()
        {
            var currentDateTime = DateTime.Now;

            // Plan maximum two days ahead
            var maxStartDateTime = currentDateTime.AddDays(2);

            // Check if startDatetime is within limit
            if (StartPickupDateTime > maxStartDateTime) return false;

            // Check if startDateTime and endDateTime are on the same date
            if (DateOnly.FromDateTime(StartPickupDateTime) != DateOnly.FromDateTime(EndPickupDateTime)) return false;

            // Check if startDatetime is not ahead of endpickupDateTime
            if (StartPickupDateTime >= EndPickupDateTime) return false;

            // Check if startDateTime is not before current time
            if (StartPickupDateTime < currentDateTime) return false;

            return true;
        }
    }
}
