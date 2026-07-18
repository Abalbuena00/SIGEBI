namespace SIGEBI.Application.Features.Catalogo;

public sealed class AgregarCategoriaRecursoCommand
{
    public int RecursoBibliograficoId { get; init; }

    public int CategoriaId { get; init; }
}