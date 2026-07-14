using SIGEBI.Application.Abstractions.Archivos;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarImagenRecursoBibliograficoCommand
{
    public int RecursoBibliograficoId { get; init; }

    public TipoImagenRecursoBibliografico TipoImagen { get; init; }

    public string NombreArchivo { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;

    public long TamanoBytes { get; init; }

    public Stream Contenido { get; init; } = Stream.Null;
}