using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomFieldListAndNumberRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ListValues",
                table: "CustomFields",
                type: "TEXT",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaxValue",
                table: "CustomFields",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinValue",
                table: "CustomFields",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Precision",
                table: "CustomFields",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListValues",
                table: "CustomFields");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "CustomFields");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "CustomFields");

            migrationBuilder.DropColumn(
                name: "Precision",
                table: "CustomFields");
        }
    }
}
