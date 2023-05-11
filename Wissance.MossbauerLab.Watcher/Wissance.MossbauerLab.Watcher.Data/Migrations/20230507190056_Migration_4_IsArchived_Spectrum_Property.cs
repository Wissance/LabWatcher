using Microsoft.EntityFrameworkCore.Migrations;

namespace Wissance.MossbauerLab.Watcher.Data.Migrations
{
    public partial class Migration_4_IsArchived_Spectrum_Property : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Wissance.MossbauerLab.Spectrum",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Wissance.MossbauerLab.Spectrum");
        }
    }
}
