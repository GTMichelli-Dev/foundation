using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddScaleCoordinatesAndMobileLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Scales",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Scales",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MobileRangeMeters",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 50);

            migrationBuilder.AddColumn<bool>(
                name: "MobileRequireLocation",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MobileRangeMeters", "MobileRequireLocation" },
                values: new object[] { 50, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Scales");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Scales");

            migrationBuilder.DropColumn(
                name: "MobileRangeMeters",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "MobileRequireLocation",
                table: "AppSetup");
        }
    }
}
