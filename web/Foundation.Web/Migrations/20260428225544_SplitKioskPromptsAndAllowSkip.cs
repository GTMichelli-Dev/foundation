using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class SplitKioskPromptsAndAllowSkip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PromptKioskLocation",
                table: "AppSetup",
                newName: "PromptKioskLocationOnInbound");

            migrationBuilder.RenameColumn(
                name: "PromptKioskCustomer",
                table: "AppSetup",
                newName: "PromptKioskCustomerOnInbound");

            migrationBuilder.RenameColumn(
                name: "PromptKioskCommodity",
                table: "AppSetup",
                newName: "PromptKioskCommodityOnInbound");

            migrationBuilder.AddColumn<bool>(
                name: "AllowSkipCarrier",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSkipCommodity",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSkipCustomer",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSkipDestination",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSkipLocation",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSkipTruckId",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "PromptKioskCommodityOnOutbound",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PromptKioskCustomerOnOutbound",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PromptKioskLocationOnOutbound",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AllowSkipCarrier", "AllowSkipCommodity", "AllowSkipCustomer", "AllowSkipDestination", "AllowSkipLocation", "AllowSkipTruckId", "PromptKioskCommodityOnOutbound", "PromptKioskCustomerOnOutbound", "PromptKioskLocationOnOutbound" },
                values: new object[] { true, true, true, true, true, true, false, false, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowSkipCarrier",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "AllowSkipCommodity",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "AllowSkipCustomer",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "AllowSkipDestination",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "AllowSkipLocation",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "AllowSkipTruckId",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "PromptKioskCommodityOnOutbound",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "PromptKioskCustomerOnOutbound",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "PromptKioskLocationOnOutbound",
                table: "AppSetup");

            migrationBuilder.RenameColumn(
                name: "PromptKioskLocationOnInbound",
                table: "AppSetup",
                newName: "PromptKioskLocation");

            migrationBuilder.RenameColumn(
                name: "PromptKioskCustomerOnInbound",
                table: "AppSetup",
                newName: "PromptKioskCustomer");

            migrationBuilder.RenameColumn(
                name: "PromptKioskCommodityOnInbound",
                table: "AppSetup",
                newName: "PromptKioskCommodity");
        }
    }
}
