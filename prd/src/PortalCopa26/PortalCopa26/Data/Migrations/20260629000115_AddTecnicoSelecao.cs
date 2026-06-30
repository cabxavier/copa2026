using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalCopa26.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTecnicoSelecao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tecnico",
                table: "Selecoes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tecnico",
                table: "Selecoes");
        }
    }
}
