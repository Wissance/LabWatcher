using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wissance.MossbauerLab.Watcher.Data.Migrations
{
    public partial class Migration_1_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wissance.MossbauerLab.Spectrum",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    First = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Last = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wissance.MossbauerLab.Spectrum", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wissance.MossbauerLab.Spectrum_Name",
                table: "Wissance.MossbauerLab.Spectrum",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wissance.MossbauerLab.Spectrum");
        }
    }
}
