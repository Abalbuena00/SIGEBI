namespace SIGEBI.Application.Features.Catalogo;

public sealed class ConsultarCatalogoQuery
{
    public string? Titulo { get; init; }

    public string? Autor { get; init; }

    public string? Categoria { get; init; }

    public bool? Disponible { get; init; }
}