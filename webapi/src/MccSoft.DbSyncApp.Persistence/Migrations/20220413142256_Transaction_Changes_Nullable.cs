using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MccSoft.DbSyncApp.Persistence.Migrations
{
    public partial class Transaction_Changes_Nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<object>(
                name: "Changes",
                table: "Transactions",
                type: "json",
                nullable: true,
                oldClrType: typeof(object),
                oldType: "json");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<object>(
                name: "Changes",
                table: "Transactions",
                type: "json",
                nullable: false,
                oldClrType: typeof(object),
                oldType: "json",
                oldNullable: true);
        }
    }
}
