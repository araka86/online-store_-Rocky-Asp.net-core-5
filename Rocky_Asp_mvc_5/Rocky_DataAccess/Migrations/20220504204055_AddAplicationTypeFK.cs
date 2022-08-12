using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky_DataAccess.Migrations
{
    public partial class AddAplicationTypeFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AplicationId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Product_AplicationId",
                table: "Product",
                column: "AplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AplicationType_AplicationId",
                table: "Product",
                column: "AplicationId",
                principalTable: "AplicationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AplicationType_AplicationId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_AplicationId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "AplicationId",
                table: "Product");
        }
    }
}
