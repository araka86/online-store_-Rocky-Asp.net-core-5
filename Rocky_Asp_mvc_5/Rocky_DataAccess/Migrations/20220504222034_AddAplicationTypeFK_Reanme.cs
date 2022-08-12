using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky_DataAccess.Migrations
{
    public partial class AddAplicationTypeFK_Reanme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AplicationType_AplicationId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "AplicationId",
                table: "Product",
                newName: "AplicationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_AplicationId",
                table: "Product",
                newName: "IX_Product_AplicationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AplicationType_AplicationTypeId",
                table: "Product",
                column: "AplicationTypeId",
                principalTable: "AplicationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AplicationType_AplicationTypeId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "AplicationTypeId",
                table: "Product",
                newName: "AplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_AplicationTypeId",
                table: "Product",
                newName: "IX_Product_AplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AplicationType_AplicationId",
                table: "Product",
                column: "AplicationId",
                principalTable: "AplicationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
