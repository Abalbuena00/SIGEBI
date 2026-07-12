namespace SIGEBI.Application.DTOs.Catalogo;

public sealed class RecursoBibliograficoDto
{
    public int Id { get; set; }

    public string CodigoInterno { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public string? Isbn { get; set; }

    public string? Editorial { get; set; }

    public int? AnioPublicacion { get; set; }

    public string? Edicion { get; set; }

    public string? ImagenPortadaUrl { get; set; }

    public string? ImagenContraportadaUrl { get; set; }

    public int CantidadEjemplares { get; set; }

    public int CantidadEjemplaresDisponibles { get; set; }
}   