namespace SIGEBI.Application.Features.Catalogo;

public sealed class ActualizarRecursoBibliograficoCommand
{
    public int RecursoBibliograficoId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public string Titulo { get; init; } = string.Empty;

    public string? Isbn { get; init; }

    public string? Editorial { get; init; }

    public int? AnioPublicacion { get; init; }

    public string? Edicion { get; init; }
}