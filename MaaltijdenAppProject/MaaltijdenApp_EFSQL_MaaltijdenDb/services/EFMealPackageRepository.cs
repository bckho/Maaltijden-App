using MaaltijdenApp_Core.Models;
using MaaltijdenApp_Core.services;
using Microsoft.EntityFrameworkCore;

namespace MaaltijdenApp_EFSQL_MaaltijdenDb.services
{
    public class EFMealPackageRepository : IMealPackageRepository
    {
        private readonly MaaltijdenAppDbContext _dbContext;

        public EFMealPackageRepository(MaaltijdenAppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<MealPackage> Create(MealPackage mealPackage)
        {
            // Check if planning date are valid.
            if (!mealPackage.PickUpDateIsValid())
            {
                throw new Exception("Opgegeven datums zijn ongeldig. Check of de start- en einddatums en tijden kloppen en dat de startdatum niet verder dan twee dagen vooruit is.");
            }

            var canteen = await _dbContext.Canteens.FirstOrDefaultAsync(c => c.Id.Equals(mealPackage.CanteenId)) ?? throw new Exception("Kantine niet gevonden.");

            // Check if submitted canteen can serve hot dinners when MealType is dinner.
            if (mealPackage.IsHotMeal && !canteen.CanServeHotDinnerMeals)
            {
                throw new Exception("Opgegeven kantine mag geen warme avondmaaltijden serveren.");
            }

            try
            {
                var result = await _dbContext.MealPackages.AddAsync(mealPackage);

                await _dbContext.SaveChangesAsync();

                return result.Entity;
            }
            catch (Exception)
            {
                throw new Exception("Maaltijd pakket kan niet aangemaakt worden, probeer het later opnieuw.");
            }
        }

        public async Task<MealPackage> CreateStudentReservation(Guid mealPackageId, Guid studentId)
        {
            var mealPackage = await _dbContext.MealPackages.Include(m => m.ReservedByStudent).Include(m => m.ProductsIndicator).FirstOrDefaultAsync(m => m.Id.Equals(mealPackageId));

            // Check if MealPackage exists
            if (mealPackage == null) throw new Exception("Maaltijdpakket niet gevonden.");
            
            // Check if MealPackage is already reserved by another student
            if (mealPackage.ReservedByStudent != null) throw new Exception("Maaltijdpakket is helaas al gereserveerd, probeer een ander maaltijdpakket te reserveren.");

            var student = await _dbContext.Students.Include(s => s.ReservedMealPackages).FirstOrDefaultAsync(s => s.Id.Equals(studentId));

            // Check if student's no-show count is already on 2
            if (student != null && student.NoShowCount >= 2) throw new Exception("Maaltijdpakketten mogen niet meer gereserveerd worden door de student omdat het aantal no-shows teveel zijn.");

            if (student == null) throw new Exception("Student niet gevonden.");

            // Check if student already has a different reservation on the same pickup date
            var mealPackagePickupDate = mealPackage.StartPickupDateTime.Date;

            var mealPackageSameDate = student.ReservedMealPackages.FirstOrDefault(m => m.StartPickupDateTime.Date == mealPackagePickupDate);

            if (mealPackageSameDate != null) throw new Exception("Meerdere maaltijdpakket reserveringen op dezelfde ophaaldag is niet toegestaan.");

            // Check if student's age is atleast 18 if MealPackage is 18+.
            var isEighteenPlusAtPickupDate = student.IsEighteenPlusAtDate(mealPackagePickupDate);
            if (isEighteenPlusAtPickupDate && student.GetAge() < 18) throw new Exception("Student is niet 18 jaar of ouder, maaltijdpakket bevat alcoholistische product(en)");

            // If checks are passed, reserve MealPackage for designated student
            mealPackage.ReservedByStudent = student;

            var updated = _dbContext.MealPackages.Update(mealPackage);

            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<MealPackage> Delete(Guid mealPackageId)
        {
            // Only without reservation
            var result = await _dbContext.MealPackages.Include(m => m.ReservedByStudent).FirstOrDefaultAsync(m => m.Id.Equals(mealPackageId));

            if (result == null) throw new Exception("Maaltijdpakket niet gevonden.");

            if (result.ReservedByStudent != null) throw new Exception("Maaltijdpakket is al gereserveerd.");

            _dbContext.MealPackages.Remove(result);

            await _dbContext.SaveChangesAsync();

            return result;
        }

        public IQueryable<MealPackage>? GetAllFutureByLocationAndMealType(string? location, MealTypeEnum? mealType)
        {
            if (location != null && mealType == null)
            {
                var results = _dbContext.MealPackages
                     .Include(m => m.Canteen)
                     .Include(m => m.ProductsIndicator)
                     .Include(m => m.Canteen)
                     .Include(m => m.ReservedByStudent)
                     .Where(m => m.Canteen.Location.Equals(location) && m.StartPickupDateTime >= DateTime.Today)
                     .OrderBy(m => m.StartPickupDateTime);

                return results;
            }

            if (location == null && mealType != null)
            {
                var results = _dbContext.MealPackages
                    .Include(m => m.Canteen)
                    .Include(m => m.ProductsIndicator)
                    .Include(m => m.Canteen)
                    .Include(m => m.ReservedByStudent)
                    .Where(m => m.MealType.Equals(mealType) && m.StartPickupDateTime >= DateTime.Today)
                    .OrderBy(m => m.StartPickupDateTime);

                return results;
            }

            if (location != null && mealType != null)
            {
                var results = _dbContext.MealPackages
                    .Include(m => m.Canteen)
                    .Include(m => m.ProductsIndicator)
                    .Include(m => m.Canteen)
                    .Include(m => m.ReservedByStudent)
                    .Where(m => m.Canteen.Location.Equals(location) && m.MealType.Equals(mealType) && m.StartPickupDateTime >= DateTime.Today)
                    .OrderBy(m => m.StartPickupDateTime);

                return results;
            }

            return null;
        }

        public IQueryable<MealPackage> GetAllFuturePlanned() =>
            _dbContext.MealPackages.Include(m => m.Canteen)
            .Include(m => m.ProductsIndicator)
            .Include(m => m.ReservedByStudent)
            .Where(m => m.StartPickupDateTime.Date >= DateTime.Today)
            .OrderBy(m => m.StartPickupDateTime);

        public IQueryable<MealPackage> GetAllNonReserved() =>
            _dbContext.MealPackages.Include(m => m.Canteen)
            .Include(m => m.ProductsIndicator)
            .Include(m => m.ReservedByStudent)
            .Where(m => m.StartPickupDateTime >= DateTime.Today && m.ReservedByStudent == null)
            .OrderBy(m => m.StartPickupDateTime);

        public IQueryable<MealPackage> GetAllReserved() =>
            _dbContext.MealPackages.Include(m => m.Canteen)
            .Include(m => m.ProductsIndicator)
            .Include(m => m.ReservedByStudent)
            .Where(m => m.StartPickupDateTime >= DateTime.Today && m.ReservedByStudent != null)
            .OrderBy(m => m.StartPickupDateTime);

        public IQueryable<MealPackage> GetReservedByStudentId(Guid studentId) =>
            _dbContext.MealPackages.Include(m => m.Canteen)
            .Include(m => m.ProductsIndicator)
            .Include(m => m.ReservedByStudent)
            .Where(m => m.StartPickupDateTime >= DateTime.Today && m.ReservedByStudentId == studentId)
            .OrderBy(m => m.StartPickupDateTime);

        public async Task<MealPackage?> GetById(Guid mealPackageId) =>
             await _dbContext.MealPackages.Include(m => m.Canteen)
            .Include(m => m.ProductsIndicator)
            .Include(m => m.ReservedByStudent)
            .FirstOrDefaultAsync(m => m.Id.Equals(mealPackageId));


        public async Task<MealPackage> Update(MealPackage mealPackage)
        {
            // Only without reservation
            var result = await _dbContext.MealPackages.Include(m => m.ReservedByStudent).FirstOrDefaultAsync(m => m.Id.Equals(mealPackage.Id));

            if (result == null) throw new Exception("Maaltijdpakket niet gevonden.");

            if (result.ReservedByStudent != null) throw new Exception("Maaltijdpakket is al gereserveerd.");

            _dbContext.MealPackages.Update(result);

            await _dbContext.SaveChangesAsync();

            return result;

            throw new NotImplementedException();
        }
    }
}
