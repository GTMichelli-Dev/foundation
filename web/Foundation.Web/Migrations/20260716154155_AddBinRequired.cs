using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddBinRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BinRequired",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                column: "BinRequired",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinRequired",
                table: "AppSetup");
        }
    }
}
