using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class CardInboundPrintForRfidScans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the new setting before dropping the old one, so a site that
            // chose to print card inbound tickets keeps printing them. Every
            // other site lands on the new default, RFID scans only.
            migrationBuilder.AddColumn<string>(
                name: "CardInboundPrint",
                table: "AppSetup",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                defaultValue: "SkipRfid");

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                column: "CardInboundPrint",
                value: "SkipRfid");

            migrationBuilder.Sql("UPDATE AppSetup SET CardInboundPrint = 'Print' WHERE PrintInboundForCard = 1;");

            migrationBuilder.DropColumn(
                name: "PrintInboundForCard",
                table: "AppSetup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PrintInboundForCard",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                column: "PrintInboundForCard",
                value: false);

            migrationBuilder.Sql("UPDATE AppSetup SET PrintInboundForCard = 1 WHERE CardInboundPrint = 'Print';");

            migrationBuilder.DropColumn(
                name: "CardInboundPrint",
                table: "AppSetup");
        }
    }
}
