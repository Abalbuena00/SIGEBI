namespace SIGEBI.Application.DTOs.Catalogo;

public sealed class ResultadoImportacionRecursosBibliograficosDto
{
    public int TotalFilas { get; init; }
    public int RecursosCreados { get; init; }
    public int RecursosOmitidos { get; init; }
    public int EjemplaresCreados { get; init; }
    public int AutoresCreados { get; init; }
    public int CategoriasCreadas { get; init; }
    public int RelacionesAutoresCreadas { get; init; }
    public int RelacionesCategoriasCreadas { get; init; }
    public IReadOnlyList<DetalleImportacionRecursoBibliograficoDto> Detalles { get; init; } = [];
}
