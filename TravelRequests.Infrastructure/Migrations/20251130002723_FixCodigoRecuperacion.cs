using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelRequests.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCodigoRecuperacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CodigosRecuperacion_UsuarioId",
                table: "CodigosRecuperacion");

            migrationBuilder.CreateIndex(
                name: "IX_CodigosRecuperacion_UsuarioId",
                table: "CodigosRecuperacion",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CodigosRecuperacion_UsuarioId",
                table: "CodigosRecuperacion");

            migrationBuilder.CreateIndex(
                name: "IX_CodigosRecuperacion_UsuarioId",
                table: "CodigosRecuperacion",
                column: "UsuarioId",
                unique: true);
        }
    }
}
