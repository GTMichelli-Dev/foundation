using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldOrdering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FieldOrderCarrier",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FieldOrderCommodity",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FieldOrderCustomer",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FieldOrderDestination",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FieldOrderLocation",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FieldOrderNotes",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FieldOrderTruckId",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FieldOrderCarrier", "FieldOrderCommodity", "FieldOrderCustomer", "FieldOrderDestination", "FieldOrderLocation", "FieldOrderNotes", "FieldOrderTruckId" },
                values: new object[] { 30, 10, 20, 60, 50, 70, 40 });

            // Existing installs: give every AppSetup row the historical form
            // order (Commodity first … Notes last), not the column default 0.
            migrationBuilder.Sql(@"
                UPDATE AppSetup SET
                    FieldOrderCommodity = 10, FieldOrderCustomer = 20,
                    FieldOrderCarrier = 30, FieldOrderTruckId = 40,
                    FieldOrderLocation = 50, FieldOrderDestination = 60,
                    FieldOrderNotes = 70
                WHERE FieldOrderCommodity = 0 AND FieldOrderCustomer = 0
                  AND FieldOrderCarrier = 0 AND FieldOrderTruckId = 0
                  AND FieldOrderLocation = 0 AND FieldOrderDestination = 0
                  AND FieldOrderNotes = 0;");

            // Custom fields historically rendered after the standard fields.
            // Rebase their SortOrder onto the shared scale (past Notes at 70)
            // preserving their relative order, so nothing moves visually until
            // the admin chooses to interleave.
            migrationBuilder.Sql("UPDATE CustomFields SET SortOrder = SortOrder * 10 + 100;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldOrderCarrier",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "FieldOrderCommodity",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "FieldOrderCustomer",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "FieldOrderDestination",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "FieldOrderLocation",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "FieldOrderNotes",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "FieldOrderTruckId",
                table: "AppSetup");
        }
    }
}
