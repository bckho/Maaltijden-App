﻿using MaaltijdenApp_Core.Models;
using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_WebApp.Models
{
    public class MealPackageCreate
    {
        public ICollection<Product> AvailableProducts { get; set; } = null!;

        public bool CanServeHotMeals { get; set; } = false;

        [Required(ErrorMessage = "Ophaaldatum en tijd verplicht.")]
        public DateTime StartPickupDateTime { get; set; }

        [Required(ErrorMessage = "Einde ophaaldatum en tijd verplicht.")]
        public DateTime EndPickupDateTime { get; set; }

        [Required(ErrorMessage = "Prijs is verplicht.")]
        //[Range(0, 1000, ErrorMessage = "Prijs is minimaal 0.00 - 1000.00.")]
        //[DataType(DataType.Currency)]
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

            // Check if startDatetime is not ahead of endpickupDateTime
            if (StartPickupDateTime > EndPickupDateTime) return false;

            // Check if startDateTime is not before current time
            if (StartPickupDateTime < currentDateTime) return false;

            if (EndPickupDateTime == StartPickupDateTime) return false;

            return true;
        }
    }
}
