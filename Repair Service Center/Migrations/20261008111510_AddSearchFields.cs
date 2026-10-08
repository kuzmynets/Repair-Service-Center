using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repair_Service_Center.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExperienceYears",
                table: "Technicians",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceBrand",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceModel",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CustomerEmail", "DeviceBrand", "DeviceModel" },
                values: new object[] { "anna.shevchenko@example.com", "Samsung", "Galaxy A54" });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CustomerEmail", "DeviceBrand", "DeviceModel" },
                values: new object[] { null, "Lenovo", "IdeaPad 5" });

            migrationBuilder.UpdateData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 1,
                column: "ExperienceYears",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 2,
                column: "ExperienceYears",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: 3,
                column: "ExperienceYears",
                value: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceYears",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeviceBrand",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeviceModel",
                table: "Orders");
        }
    }
}
