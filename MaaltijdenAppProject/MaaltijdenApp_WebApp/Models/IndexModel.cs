using MaaltijdenApp_Core.Models;

namespace MaaltijdenApp_WebApp.Models
{
    public class IndexModel
    {
        public IQueryable<MealPackage>? MealPackages { get; set; }

        public CityEnum? CanteenCity { get; set; }

        public bool? IsReserved { get; set; }
    }
}
