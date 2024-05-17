using MaaltijdenApp_Core.Models;
using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_WebApp.Models
{
    public class InitializeMealPackage
    {
        // DateOnly and TimeOnly HTML input binding are supported upward of .NET 7 🙁.

        [Required(ErrorMessage = "Ophaaldatum en tijd verplicht.")]
        public DateTime StartPickupDateTime { get; set; }

        [Required(ErrorMessage = "Einde ophaaldatum en tijd verplicht.")]
        public DateTime EndPickupDateTime { get; set; }

        [Required(ErrorMessage = "Prijs is verplicht.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Maaltijd type is verplicht.")]
        public MealTypeEnum MealType { get; set; }

        // Only if employee's canteen provides this option, default false.
        public bool IsHotMeal { get; set; } = false;

        [Required(ErrorMessage = "18+ indicator is verplicht.")]
        public bool IsEighteenPlusOnly { get; set; }

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
