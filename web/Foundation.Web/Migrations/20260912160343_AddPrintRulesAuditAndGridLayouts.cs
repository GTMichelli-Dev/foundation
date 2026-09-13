using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPrintRulesAuditAndGridLayouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BoldTicketText",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "HideKioskWeighInForCards",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "KioskPrintInbound",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "KioskPrintOutbound",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrintInboundForCard",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrintWhenNoRuleMatches",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EntityKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Field = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OldValue = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    NewValue = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GridLayouts",
                columns: table => new
                {
                    GridKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StateJson = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridLayouts", x => x.GridKey);
                });

            migrationBuilder.CreateTable(
                name: "PrintRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Leg = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Customer = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Carrier = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TruckId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Commodity = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Destination = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CustomFieldId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomFieldValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Print = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintRules", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BoldTicketText", "HideKioskWeighInForCards", "KioskPrintInbound", "KioskPrintOutbound", "PrintInboundForCard", "PrintWhenNoRuleMatches" },
                values: new object[] { true, false, true, true, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityKey",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "GridLayouts");

            migrationBuilder.DropTable(
                name: "PrintRules");

            migrationBuilder.DropColumn(
                name: "BoldTicketText",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "HideKioskWeighInForCards",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "KioskPrintInbound",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "KioskPrintOutbound",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "PrintInboundForCard",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "PrintWhenNoRuleMatches",
                table: "AppSetup");
        }
    }
}
