using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgMovilWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ImagenUnicaPorProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Imagenes_ProductoId",
                table: "Imagenes");

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_ProductoId",
                table: "Imagenes",
                column: "ProductoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Imagenes_ProductoId",
                table: "Imagenes");

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_ProductoId",
                table: "Imagenes",
                column: "ProductoId");
        }
    }
}
