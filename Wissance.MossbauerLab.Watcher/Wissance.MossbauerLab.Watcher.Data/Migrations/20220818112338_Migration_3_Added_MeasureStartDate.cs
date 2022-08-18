using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wissance.MossbauerLab.Watcher.Data.Migrations
{
    public partial class Migration_3_Added_MeasureStartDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MeasureStartDate",
                table: "Wissance.MossbauerLab.Spectrum",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeasureStartDate",
                table: "Wissance.MossbauerLab.Spectrum");
        }
    }
}
