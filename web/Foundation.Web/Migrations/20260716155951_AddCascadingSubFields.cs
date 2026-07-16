using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadingSubFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentField",
                table: "CustomFields",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomFieldListValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomFieldId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldListValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomFieldListValues_CustomFields_CustomFieldId",
                        column: x => x.CustomFieldId,
                        principalTable: "CustomFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomFieldListValues_CustomFieldId_ParentValue_Value",
                table: "CustomFieldListValues",
                columns: new[] { "CustomFieldId", "ParentValue", "Value" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomFieldListValues");

            migrationBuilder.DropColumn(
                name: "ParentField",
                table: "CustomFields");
        }
    }
}
