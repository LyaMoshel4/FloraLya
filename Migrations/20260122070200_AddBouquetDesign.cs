using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LyaShop.Migrations
{
    /// <inheritdoc />
    public partial class AddBouquetDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BouquetDesignHtml",
                table: "Bouquet",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BouquetDesignHtml",
                table: "Bouquet");
        }
    }
}
