namespace SIGEBI.Application.DTOs.Catalogo;

public sealed class CategoriaDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
}