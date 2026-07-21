namespace SIGEBI.Application.Features.Catalogo;

public sealed class ConsultarCatalogoQuery
{
    public string? Titulo { get; init; }

    public string? Autor { get; init; }

    public string? Categoria { get; init; }

    public bool? Disponible { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}