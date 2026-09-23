using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Repair_Service_Center.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplexityLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplexityLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technicians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technicians", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceListItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceTypeId = table.Column<int>(type: "int", nullable: false),
                    RepairTypeId = table.Column<int>(type: "int", nullable: false),
                    ComplexityLevelId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceListItems_ComplexityLevels_ComplexityLevelId",
                        column: x => x.ComplexityLevelId,
                        principalTable: "ComplexityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceListItems_DeviceTypes_DeviceTypeId",
                        column: x => x.DeviceTypeId,
                        principalTable: "DeviceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceListItems_RepairTypes_RepairTypeId",
                        column: x => x.RepairTypeId,
                        principalTable: "RepairTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProblemDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TechnicianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "ComplexityLevels",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Budget" },
                    { 2, "Mid-range" },
                    { 3, "Premium" }
                });

            migrationBuilder.InsertData(
                table: "DeviceTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Smartphone" },
                    { 2, "Laptop" },
                    { 3, "Tablet" },
                    { 4, "Washing machine" },
                    { 5, "Refrigerator" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "CustomerName", "CustomerPhone", "ProblemDescription", "Status", "TechnicianId", "TotalCost" },
                values: new object[] { 2, new DateTime(2026, 9, 22, 14, 15, 0, 0, DateTimeKind.Unspecified), "Dmytro Tkachenko", "+380672223344", "Laptop turns off after 10 minutes of work", "Pending", null, null });

            migrationBuilder.InsertData(
                table: "RepairTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Full check of the device to find the problem", "Diagnostics" },
                    { 2, "Replacement of a broken display module", "Screen replacement" },
                    { 3, "Replacement of an old or swollen battery", "Battery replacement" },
                    { 4, "Dust cleaning and thermal paste replacement", "Cleaning" },
                    { 5, "Reinstall of the operating system and drivers", "Software reinstall" }
                });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "FullName", "Phone", "Specialization" },
                values: new object[,]
                {
                    { 1, "Oleksandr Kovalenko", "+380671234567", "Smartphones and tablets" },
                    { 2, "Iryna Melnyk", "+380502345678", "Laptops and computers" },
                    { 3, "Petro Bondarenko", "+380933456789", "Home appliances" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "CustomerName", "CustomerPhone", "ProblemDescription", "Status", "TechnicianId", "TotalCost" },
                values: new object[] { 1, new DateTime(2026, 9, 20, 10, 30, 0, 0, DateTimeKind.Unspecified), "Anna Shevchenko", "+380501112233", "Broken screen after a fall", "Accepted", 1, 1500m });

            migrationBuilder.InsertData(
                table: "PriceListItems",
                columns: new[] { "Id", "ComplexityLevelId", "DeviceTypeId", "DurationMinutes", "Price", "RepairTypeId" },
                values: new object[,]
                {
                    { 1, 1, 1, 30, 200m, 1 },
                    { 2, 1, 1, 60, 1500m, 2 },
                    { 3, 3, 1, 90, 4500m, 2 },
                    { 4, 2, 1, 45, 900m, 3 },
                    { 5, 2, 2, 60, 800m, 4 },
                    { 6, 1, 2, 60, 500m, 5 },
                    { 7, 1, 4, 45, 350m, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TechnicianId",
                table: "Orders",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItems_ComplexityLevelId",
                table: "PriceListItems",
                column: "ComplexityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItems_DeviceTypeId",
                table: "PriceListItems",
                column: "DeviceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItems_RepairTypeId",
                table: "PriceListItems",
                column: "RepairTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "PriceListItems");

            migrationBuilder.DropTable(
                name: "Technicians");

            migrationBuilder.DropTable(
                name: "ComplexityLevels");

            migrationBuilder.DropTable(
                name: "DeviceTypes");

            migrationBuilder.DropTable(
                name: "RepairTypes");
        }
    }
}
