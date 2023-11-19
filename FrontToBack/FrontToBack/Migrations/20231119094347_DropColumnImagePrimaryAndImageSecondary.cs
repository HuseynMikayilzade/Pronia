using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrontToBack.Migrations
{
    /// <inheritdoc />
    public partial class DropColumnImagePrimaryAndImageSecondary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ImagePrimary",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageSecondary",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages");

            migrationBuilder.AddColumn<string>(
                name: "ImagePrimary",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageSecondary",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId",
                unique: true);
        }
    }
}
