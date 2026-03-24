using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LyaShop.Migrations
{
    /// <inheritdoc />
    public partial class FinalTouch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Bouquet");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Bouquet");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Bouquet");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Flower",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Bouquet",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.CreateTable(
                name: "BouquetItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BouquetId = table.Column<int>(type: "int", nullable: false),
                    FlowerId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BouquetItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BouquetItem_Bouquet_BouquetId",
                        column: x => x.BouquetId,
                        principalTable: "Bouquet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BouquetItem_Flower_FlowerId",
                        column: x => x.FlowerId,
                        principalTable: "Flower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BouquetItem_BouquetId",
                table: "BouquetItem",
                column: "BouquetId");

            migrationBuilder.CreateIndex(
                name: "IX_BouquetItem_FlowerId",
                table: "BouquetItem",
                column: "FlowerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BouquetItem");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Flower",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Bouquet",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "CustomerID",
                table: "Bouquet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Bouquet",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Bouquet",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
