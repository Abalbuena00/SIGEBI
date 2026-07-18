namespace SIGEBI.Application.Features.Catalogo;

public sealed class QuitarCategoriaRecursoCommand
{
    public int RecursoBibliograficoId { get; init; }

    public int CategoriaId { get; init; }
}
