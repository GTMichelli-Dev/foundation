using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiScale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InScale",
                table: "Transactions",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutScale",
                table: "Transactions",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Scales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    HardwareId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scales", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scales_Name",
                table: "Scales",
                column: "Name",
                unique: true);

            // Existing installs: carry the single configured scale forward as
            // the first named scale so weighing keeps working unchanged.
            // (Fresh databases get their default scale from DbInitializer.)
            migrationBuilder.Sql(@"
                INSERT INTO Scales (Name, HardwareId, SortOrder, Active)
                SELECT 'Scale 1', NULLIF(ScaleId, ''), 10, 1
                FROM AppSetup
                LIMIT 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scales");

            migrationBuilder.DropColumn(
                name: "InScale",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OutScale",
                table: "Transactions");
        }
    }
}
