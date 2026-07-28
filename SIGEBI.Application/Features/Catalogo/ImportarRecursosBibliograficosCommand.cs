namespace SIGEBI.Application.Features.Catalogo;

public sealed class ImportarRecursosBibliograficosCommand
{
    public int UsuarioResponsableId { get; init; }
    public string NombreArchivo { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long TamanoBytes { get; init; }
    public Stream Contenido { get; init; } = Stream.Null;
}
