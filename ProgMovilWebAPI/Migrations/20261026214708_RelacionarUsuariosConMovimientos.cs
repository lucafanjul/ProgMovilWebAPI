using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgMovilWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class RelacionarUsuariosConMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "SalidasProductos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "IngresosProductos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalidasProductos_UsuarioId",
                table: "SalidasProductos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosProductos_UsuarioId",
                table: "IngresosProductos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngresosProductos_Usuarios_UsuarioId",
                table: "IngresosProductos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalidasProductos_Usuarios_UsuarioId",
                table: "SalidasProductos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngresosProductos_Usuarios_UsuarioId",
                table: "IngresosProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_SalidasProductos_Usuarios_UsuarioId",
                table: "SalidasProductos");

            migrationBuilder.DropIndex(
                name: "IX_SalidasProductos_UsuarioId",
                table: "SalidasProductos");

            migrationBuilder.DropIndex(
                name: "IX_IngresosProductos_UsuarioId",
                table: "IngresosProductos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "SalidasProductos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "IngresosProductos");
        }
    }
}
