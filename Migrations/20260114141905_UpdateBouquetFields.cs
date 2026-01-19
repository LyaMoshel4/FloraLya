using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LyaShop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBouquetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flower_Bouquet_BouquetId",
                table: "Flower");
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
            migrationBuilder.DropIndex(
                name: "IX_Flower_BouquetId",
                table: "Flower");

            migrationBuilder.DropColumn(
                name: "BouquetId",
                table: "Flower");

            migrationBuilder.RenameColumn(
                name: "NameByItem",
                table: "Bouquet",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "FlowerInBouquet",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Flower",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Bouquet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Bouquet",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "FlowerInBouquet");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Bouquet");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Bouquet");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Bouquet",
                newName: "NameByItem");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Flower",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "BouquetId",
                table: "Flower",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flower_BouquetId",
                table: "Flower",
                column: "BouquetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flower_Bouquet_BouquetId",
                table: "Flower",
                column: "BouquetId",
                principalTable: "Bouquet",
                principalColumn: "Id");
        }
    }
}
