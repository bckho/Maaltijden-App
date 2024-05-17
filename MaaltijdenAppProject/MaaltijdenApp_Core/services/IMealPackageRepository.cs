using MaaltijdenApp_Core.Models;

namespace MaaltijdenApp_Core.services
{
    public interface IMealPackageRepository
    {
        public Task<MealPackage> Create(MealPackage mealPackage);
        public Task<MealPackage> CreateStudentReservation(Guid mealPackageId, Guid studentId);
        public IQueryable<MealPackage>? GetAllFutureByLocationAndMealType(string? location, MealTypeEnum? mealType);
        public IQueryable<MealPackage> GetAllFuturePlanned();       // Today and future
        public IQueryable<MealPackage> GetAllReserved();
        public IQueryable<MealPackage> GetReservedByStudentId(Guid studentId);
        public IQueryable<MealPackage> GetAllNonReserved();
        public Task<MealPackage?> GetById(Guid mealPackageId);
        public Task<MealPackage> Update(MealPackage mealPackage);   // Only without reservations
        public Task<MealPackage> Delete(Guid mealPackageId);        // Only without reservations
    }
}
