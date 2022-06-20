using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MccSoft.DbSyncApp.Persistence.Migrations
{
    public partial class Box_Added_Color_Count : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Boxes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Boxes",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "Boxes");
        }
    }
}
