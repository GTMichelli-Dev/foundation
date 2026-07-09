using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSignatureCaptureSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PrintSignatureOnTicket",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SignatureMode",
                table: "AppSetup",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignaturePadId",
                table: "AppSetup",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SignatureRequired",
                table: "AppSetup",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AppSetup",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PrintSignatureOnTicket", "SignatureMode", "SignaturePadId", "SignatureRequired" },
                values: new object[] { true, "None", null, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrintSignatureOnTicket",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "SignatureMode",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "SignaturePadId",
                table: "AppSetup");

            migrationBuilder.DropColumn(
                name: "SignatureRequired",
                table: "AppSetup");
        }
    }
}
