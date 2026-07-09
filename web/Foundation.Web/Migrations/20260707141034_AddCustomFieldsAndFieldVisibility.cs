using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomFieldsAndFieldVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HideCarrier",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HideCommodity",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HideCustomer",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HideDestination",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HideLocation",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HideNotes",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HideTruckId",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CustomFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FieldType = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Required = table.Column<bool>(type: "INTEGER", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowOnTicket = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCustomValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ticket = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CustomFieldId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCustomValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionCustomValues_CustomFields_CustomFieldId",
                        column: x => x.CustomFieldId,
                        principalTable: "CustomFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionCustomValues_Transactions_Ticket",
                        column: x => x.Ticket,
                        principalTable: "Transactions",
                        principalColumn: "Ticket",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "HideCarrier", "HideCommodity", "HideCustomer", "HideDestination", "HideLocation", "HideNotes", "HideTruckId" },
                values: new object[] { false, false, false, false, false, false, false });

            migrationBuilder.CreateIndex(
                name: "IX_CustomFields_Name",
                table: "CustomFields",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCustomValues_CustomFieldId",
                table: "TransactionCustomValues",
                column: "CustomFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCustomValues_Ticket",
                table: "TransactionCustomValues",
                column: "Ticket");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCustomValues_Ticket_CustomFieldId",
                table: "TransactionCustomValues",
                columns: new[] { "Ticket", "CustomFieldId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionCustomValues");

            migrationBuilder.DropTable(
                name: "CustomFields");

            migrationBuilder.DropColumn(
                name: "HideCarrier",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "HideCommodity",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "HideCustomer",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "HideDestination",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "HideLocation",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "HideNotes",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "HideTruckId",
                table: "AppSetup");
        }
    }
}
