using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEBI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookCoverImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenContraportadaContentType",
                schema: "catalogo",
                table: "RecursosBibliograficos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenContraportadaNombreArchivo",
                schema: "catalogo",
                table: "RecursosBibliograficos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenContraportadaUrl",
                schema: "catalogo",
                table: "RecursosBibliograficos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenPortadaContentType",
                schema: "catalogo",
                table: "RecursosBibliograficos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenPortadaNombreArchivo",
                schema: "catalogo",
                table: "RecursosBibliograficos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenPortadaUrl",
                schema: "catalogo",
                table: "RecursosBibliograficos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenContraportadaContentType",
                schema: "catalogo",
                table: "RecursosBibliograficos");

            migrationBuilder.DropColumn(
                name: "ImagenContraportadaNombreArchivo",
                schema: "catalogo",
                table: "RecursosBibliograficos");

            migrationBuilder.DropColumn(
                name: "ImagenContraportadaUrl",
                schema: "catalogo",
                table: "RecursosBibliograficos");

            migrationBuilder.DropColumn(
                name: "ImagenPortadaContentType",
                schema: "catalogo",
                table: "RecursosBibliograficos");

            migrationBuilder.DropColumn(
                name: "ImagenPortadaNombreArchivo",
                schema: "catalogo",
                table: "RecursosBibliograficos");

            migrationBuilder.DropColumn(
                name: "ImagenPortadaUrl",
                schema: "catalogo",
                table: "RecursosBibliograficos");
        }
    }
}
