using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaltijdenApp_EFSQL_MaaltijdenDb.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Canteens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanServeHotDinnerMeals = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsExample = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContainsAlcohol = table.Column<bool>(type: "bit", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoShowCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CanteenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Canteens_CanteenId",
                        column: x => x.CanteenId,
                        principalTable: "Canteens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<int>(type: "int", nullable: false),
                    CanteenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartPickupDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndPickupDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MealType = table.Column<int>(type: "int", nullable: false),
                    IsHotMeal = table.Column<bool>(type: "bit", nullable: false),
                    IsEighteenPlusOnly = table.Column<bool>(type: "bit", nullable: false),
                    ReservedByStudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsNoShow = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealPackages_Canteens_CanteenId",
                        column: x => x.CanteenId,
                        principalTable: "Canteens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealPackages_Students_ReservedByStudentId",
                        column: x => x.ReservedByStudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MealPackageProduct",
                columns: table => new
                {
                    MealPackagesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsIndicatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPackageProduct", x => new { x.MealPackagesId, x.ProductsIndicatorId });
                    table.ForeignKey(
                        name: "FK_MealPackageProduct_MealPackages_MealPackagesId",
                        column: x => x.MealPackagesId,
                        principalTable: "MealPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealPackageProduct_Products_ProductsIndicatorId",
                        column: x => x.ProductsIndicatorId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Canteens",
                columns: new[] { "Id", "CanServeHotDinnerMeals", "City", "Location" },
                values: new object[,]
                {
                    { new Guid("09b1199e-f178-43f9-9a51-991b152be347"), true, 0, "Hogeschoollaan" },
                    { new Guid("5fb90f12-22df-4793-905c-f1629276bba9"), false, 0, "Lovensdijk 61" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "ContainsAlcohol", "Description", "Image", "IsExample", "Name" },
                values: new object[,]
                {
                    { new Guid("20588f72-28b0-4df5-a3f8-10aab358de01"), true, "Bier.", null, true, "Leffe Blond" },
                    { new Guid("7bcfbfa3-e200-47b5-8dbd-c98f0f30965f"), false, "Versgeperste sinasappelsap zonder conservatieven.", null, true, "Sinasappelsap" },
                    { new Guid("bcecfc2c-8a37-4297-bb35-67eee658c31f"), false, "Een klassieker! Met Unox rookworst.", null, true, "Broodje rookworst" },
                    { new Guid("e05ae46c-23d3-4bf6-b716-771f78e0127c"), false, "Lasagne met seizoensgroenten en verse pasta.", null, true, "Vegetarische lasagne" },
                    { new Guid("e381a329-205f-484c-b809-f668bc63281e"), false, "Smaakvolle kippensoep met vermicelli en zachte kipballetjes.", null, true, "Kippensoep" },
                    { new Guid("f19bada6-94ee-49eb-9da7-65f284e86487"), false, "Pittige thaise curry met kip, peulboontjes en aubergine.", null, true, "Thaise groene curry" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "BirthDate", "Email", "FirstName", "LastName", "NoShowCount" },
                values: new object[,]
                {
                    { new Guid("7ee24e32-da28-4427-9126-d219aac4f81e"), new DateTime(2000, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "h.kaasboer@student.avans.nl", "Henk", "Kaasboer", 0 },
                    { new Guid("c77cd1c8-3b4b-497d-8551-50c4d13132a0"), new DateTime(2008, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "b.boos@student.avans.nl", "Bob", "Boos", 0 }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "BirthDate", "CanteenId", "Email", "FirstName", "LastName" },
                values: new object[] { new Guid("5641e7da-1ddd-4eca-ad3d-154919eaf61c"), new DateTime(1984, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("5fb90f12-22df-4793-905c-f1629276bba9"), "a.koekenbakker@avans.nl", "Alice", "Koekenbakker" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "BirthDate", "CanteenId", "Email", "FirstName", "LastName" },
                values: new object[] { new Guid("86b2c9af-0215-492e-a037-8663fde75a51"), new DateTime(1990, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("09b1199e-f178-43f9-9a51-991b152be347"), "r.bos@avans.nl", "Rik", "Bos" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CanteenId",
                table: "Employees",
                column: "CanteenId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealPackageProduct_ProductsIndicatorId",
                table: "MealPackageProduct",
                column: "ProductsIndicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPackages_CanteenId",
                table: "MealPackages",
                column: "CanteenId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPackages_ReservedByStudentId",
                table: "MealPackages",
                column: "ReservedByStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "MealPackageProduct");

            migrationBuilder.DropTable(
                name: "MealPackages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Canteens");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
