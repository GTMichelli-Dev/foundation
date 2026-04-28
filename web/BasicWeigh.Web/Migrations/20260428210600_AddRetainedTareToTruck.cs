using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasicWeigh.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRetainedTareToTruck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RetainedTare",
                table: "Trucks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RetainedTareUpdated",
                table: "Trucks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetainedTare",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "RetainedTareUpdated",
                table: "Trucks");
        }
    }
}
