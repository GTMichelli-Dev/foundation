using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddBinInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bin",
                table: "Transactions",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSkipBin",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "FieldOrderBin",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PromptKioskBinOnInbound",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "PromptKioskBinOnOutbound",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseBinInventory",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BinAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Bin = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Commodity = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AmountLbs = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinAdjustments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BinName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    UseAtKiosk = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bins", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AllowSkipBin", "FieldOrderBin", "PromptKioskBinOnInbound", "PromptKioskBinOnOutbound", "UseBinInventory" },
                values: new object[] { true, 65, true, false, false });

            migrationBuilder.CreateIndex(
                name: "IX_BinAdjustments_Bin",
                table: "BinAdjustments",
                column: "Bin");

            migrationBuilder.CreateIndex(
                name: "IX_Bins_BinName",
                table: "Bins",
                column: "BinName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BinAdjustments");

            migrationBuilder.DropTable(
                name: "Bins");

            migrationBuilder.DropColumn(
                name: "Bin",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AllowSkipBin",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "FieldOrderBin",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "PromptKioskBinOnInbound",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "PromptKioskBinOnOutbound",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "UseBinInventory",
                table: "AppSetup");
        }
    }
}
