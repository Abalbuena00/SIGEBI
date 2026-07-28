namespace SIGEBI.Application.Abstractions.Importacion;

public sealed class FilaImportacionRecursoBibliografico
{
    public int NumeroFila { get; init; }
    public string CodigoInternoRecurso { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string? Isbn { get; init; }
    public string? Editorial { get; init; }
    public int? AnioPublicacion { get; init; }
    public string? Edicion { get; init; }
    public string? CodigoInternoEjemplar { get; init; }
    public string? EstadoFisico { get; init; }
    public IReadOnlyList<string> Autores { get; init; } = [];
    public IReadOnlyList<string> Categorias { get; init; } = [];
}
