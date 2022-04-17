using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MccSoft.DbSyncApp.Persistence.Migrations
{
    public partial class Added_Bool_Field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFull",
                table: "Box",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFull",
                table: "Box");
        }
    }
}
