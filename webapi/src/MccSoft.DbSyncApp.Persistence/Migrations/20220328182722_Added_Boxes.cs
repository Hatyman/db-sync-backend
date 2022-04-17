using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MccSoft.DbSyncApp.Persistence.Migrations
{
    public partial class Added_Boxes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoxId",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Box",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Box", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoxSale",
                columns: table => new
                {
                    BoxesId = table.Column<string>(type: "text", nullable: false),
                    SalesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxSale", x => new { x.BoxesId, x.SalesId });
                    table.ForeignKey(
                        name: "FK_BoxSale_Box_BoxesId",
                        column: x => x.BoxesId,
                        principalTable: "Box",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoxSale_Sales_SalesId",
                        column: x => x.SalesId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_BoxId",
                table: "Products",
                column: "BoxId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoxSale_SalesId",
                table: "BoxSale",
                column: "SalesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Box_BoxId",
                table: "Products",
                column: "BoxId",
                principalTable: "Box",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Box_BoxId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "BoxSale");

            migrationBuilder.DropTable(
                name: "Box");

            migrationBuilder.DropIndex(
                name: "IX_Products_BoxId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BoxId",
                table: "Products");
        }
    }
}
