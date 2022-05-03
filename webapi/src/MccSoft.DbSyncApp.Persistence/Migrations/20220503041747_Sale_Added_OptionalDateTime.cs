using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MccSoft.DbSyncApp.Persistence.Migrations
{
    public partial class Sale_Added_OptionalDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OptionalDateTime",
                table: "Sales",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionalDateTime",
                table: "Sales");
        }
    }
}
