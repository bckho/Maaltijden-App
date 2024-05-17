using MaaltijdenApp_Core.Models;
using MaaltijdenApp_EFSQL_MaaltijdenDb;

namespace MaaltijdenApp_WebAPI.Models
{
    public class MealQuery
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<MealPackage> GetMealPackages([Service] MaaltijdenAppDbContext context) =>
            context.MealPackages;
    }
}