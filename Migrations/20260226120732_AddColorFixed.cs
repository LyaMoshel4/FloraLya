using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LyaShop.Migrations
{
    /// <inheritdoc />
    public partial class AddColorFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Flower",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Flower",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Flower");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Flower");
        }
    }
}
