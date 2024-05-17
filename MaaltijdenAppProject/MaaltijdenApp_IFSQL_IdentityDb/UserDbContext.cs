using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaaltijdenApp_IFSQL_IdentityDb
{
    public class UserDbContext : IdentityDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            SeedUsersAndRoles(builder);
        }

        private void SeedUsersAndRoles(ModelBuilder builder)
        {
            var hasher = new PasswordHasher<IdentityUser>();

            // Employee users
            var employeeUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "alice.koekenbakker",
                NormalizedUserName = "alice.koekenbakker",
                Email = "a.koekenbakker@avans.nl",
                NormalizedEmail = "a.koekenbakker@avans.nl",
                LockoutEnabled = false
            };

            var employeeUser1 = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "rik.bos",
                NormalizedUserName = "rik.bos",
                Email = "r.bos@avans.nl",
                NormalizedEmail = "r.bos@avans.nl",
                LockoutEnabled = false
            };

            // Students
            var studentUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "henk.kaasboer",
                NormalizedUserName = "henk.kaasboer",
                Email = "h.kaasboer@student.avans.nl",
                NormalizedEmail = "h.kaasboer@student.avans.nl",
                LockoutEnabled = false
            };

            var studentUser1 = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "bob.boos",
                NormalizedUserName = "bob.boos",
                Email = "b.boos@student.avans.nl",
                NormalizedEmail = "b.boos@student.avans.nl",
                LockoutEnabled = false
            };

            var passhash = hasher.HashPassword(employeeUser, "test123");
            var passhash1 = hasher.HashPassword(studentUser, "test123");
            var passhash2 = hasher.HashPassword(studentUser1, "test123");
            var passhash3 = hasher.HashPassword(employeeUser1, "test123");

            employeeUser.PasswordHash = passhash;
            studentUser.PasswordHash = passhash1;
            studentUser1.PasswordHash = passhash2;
            employeeUser1.PasswordHash = passhash3;

            builder.Entity<IdentityUser>().HasData(employeeUser);
            builder.Entity<IdentityUser>().HasData(studentUser);
            builder.Entity<IdentityUser>().HasData(employeeUser1);
            builder.Entity<IdentityUser>().HasData(studentUser1);

            // Roles
            var employeeRole = new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "employee",
                ConcurrencyStamp = "1",
                NormalizedName = "employee"
            };

            var studentRole = new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "student",
                ConcurrencyStamp = "2",
                NormalizedName = "student"
            };

            builder.Entity<IdentityRole>().HasData(
                employeeRole,
                studentRole
                );

            // User Roles
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = employeeRole.Id,
                    UserId = employeeUser.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = studentRole.Id,
                    UserId = studentUser.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = studentRole.Id,
                    UserId = studentUser1.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = employeeRole.Id,
                    UserId = employeeUser1.Id
                });
        }
    }
}
