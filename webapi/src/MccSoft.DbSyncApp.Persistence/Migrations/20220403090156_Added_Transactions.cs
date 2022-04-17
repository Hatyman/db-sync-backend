using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MccSoft.DbSyncApp.Persistence.Migrations
{
    public partial class Added_Transactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxSale_Box_BoxesId",
                table: "BoxSale");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Box_BoxId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Box",
                table: "Box");

            migrationBuilder.RenameTable(
                name: "Box",
                newName: "Boxes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Boxes",
                table: "Boxes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TableName = table.Column<string>(type: "text", nullable: true),
                    Changes = table.Column<object>(type: "json", nullable: true),
                    ChangeType = table.Column<int>(type: "integer", nullable: false),
                    InstanceId = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SyncDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BoxSale_Boxes_BoxesId",
                table: "BoxSale",
                column: "BoxesId",
                principalTable: "Boxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Boxes_BoxId",
                table: "Products",
                column: "BoxId",
                principalTable: "Boxes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxSale_Boxes_BoxesId",
                table: "BoxSale");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Boxes_BoxId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Boxes",
                table: "Boxes");

            migrationBuilder.RenameTable(
                name: "Boxes",
                newName: "Box");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Box",
                table: "Box",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxSale_Box_BoxesId",
                table: "BoxSale",
                column: "BoxesId",
                principalTable: "Box",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Box_BoxId",
                table: "Products",
                column: "BoxId",
                principalTable: "Box",
                principalColumn: "Id");
        }
    }
}
