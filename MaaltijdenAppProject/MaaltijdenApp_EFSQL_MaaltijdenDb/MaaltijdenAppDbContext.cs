using MaaltijdenApp_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MaaltijdenApp_EFSQL_MaaltijdenDb
{
    public class MaaltijdenAppDbContext : DbContext
    {
        public MaaltijdenAppDbContext(DbContextOptions<MaaltijdenAppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal type for MealPackage.
            foreach (var property in modelBuilder.Model.GetEntityTypes()
               .SelectMany(t => t.GetProperties())
               .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            modelBuilder.Entity<Student>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Canteen)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.CanteenId);

            modelBuilder.Entity<MealPackage>()
                .HasOne(e => e.ReservedByStudent)
                .WithMany(e => e.ReservedMealPackages)
                .HasForeignKey(e => e.ReservedByStudentId);

            //modelBuilder.Entity<MealPackage>()
            //    .HasMany(e => e.ProductsIndicator)
            //    .WithOne();


            SeedData(modelBuilder);
        }

        public virtual DbSet<Canteen> Canteens { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<MealPackage> MealPackages { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        public void SeedData(ModelBuilder modelBuilder)
        {
            // Create canteen data
            var canteen = new Canteen { Id = Guid.NewGuid(), City = CityEnum.Breda, Location = "Lovensdijk 61", CanServeHotDinnerMeals = false };
            modelBuilder.Entity<Canteen>().HasData(canteen);

            var canteen1 = new Canteen { Id = Guid.NewGuid(), City = CityEnum.Breda, Location = "Hogeschoollaan", CanServeHotDinnerMeals = true };
            modelBuilder.Entity<Canteen>().HasData(canteen1);


            // Create Employee data
            var employee = new Employee { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Koekenbakker", BirthDate = new DateTime(1984, 6, 1), Email = "a.koekenbakker@avans.nl", CanteenId = canteen.Id };
            modelBuilder.Entity<Employee>().HasData(employee);

            var employee1 = new Employee { Id = Guid.NewGuid(), FirstName = "Rik", LastName = "Bos", BirthDate = new DateTime(1990, 12, 1), Email = "r.bos@avans.nl", CanteenId = canteen1.Id };
            modelBuilder.Entity<Employee>().HasData(employee1);


            // Create student data
            var student = new Student { Id = Guid.NewGuid(), FirstName = "Henk", LastName = "Kaasboer", BirthDate = new DateTime(2000, 2, 1), Email = "h.kaasboer@student.avans.nl" };
            modelBuilder.Entity<Student>().HasData(student);

            var student1 = new Student { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Boos", BirthDate = new DateTime(2008, 1, 12), Email = "b.boos@student.avans.nl" };
            modelBuilder.Entity<Student>().HasData(student1);


            // Create product data
            var product = new Product { Id = Guid.NewGuid(), Name = "Kippensoep", ContainsAlcohol = false, Description = "Smaakvolle kippensoep met vermicelli en zachte kipballetjes.", IsExample = true };
            var product1 = new Product { Id = Guid.NewGuid(), Name = "Thaise groene curry", ContainsAlcohol = false, Description = "Pittige thaise curry met kip, peulboontjes en aubergine.", IsExample = true };
            var product2 = new Product { Id = Guid.NewGuid(), Name = "Sinasappelsap", ContainsAlcohol = false, Description = "Versgeperste sinasappelsap zonder conservatieven.", IsExample = true };
            var product3 = new Product { Id = Guid.NewGuid(), Name = "Vegetarische lasagne", ContainsAlcohol = false, Description = "Lasagne met seizoensgroenten en verse pasta.", IsExample = true };
            var product4 = new Product { Id = Guid.NewGuid(), Name = "Broodje rookworst", ContainsAlcohol = false, Description = "Een klassieker! Met Unox rookworst.", IsExample = true };
            var product5 = new Product { Id = Guid.NewGuid(), Name = "Leffe Blond", ContainsAlcohol = true, Description = "Bier.", IsExample = true };


            modelBuilder.Entity<Product>().HasData(product);
            modelBuilder.Entity<Product>().HasData(product1);
            modelBuilder.Entity<Product>().HasData(product2);
            modelBuilder.Entity<Product>().HasData(product3);
            modelBuilder.Entity<Product>().HasData(product4);
            modelBuilder.Entity<Product>().HasData(product5);

            // Create mealpackage data
            //var mealPackage = new MealPackage { Id = Guid.NewGuid(), ProductsIndicator = new List<Product> { product, product1, product2 }, City = CityEnum.Breda, CanteenId = canteen.Id, StartPickupDateTime = new DateTime(2023, 10, 25, 17, 0, 0), EndPickupDateTime = new DateTime(2023, 10, 25, 19, 0, 0), Price = 7.45m, MealType = MealTypeEnum.Diner, IsHotMeal = true, ReservedByStudentId = student.Id };
            //modelBuilder.Entity<MealPackage>().HasData(mealPackage);
        }
    }
}