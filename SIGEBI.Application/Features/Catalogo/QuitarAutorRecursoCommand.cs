namespace SIGEBI.Application.Features.Catalogo;

public sealed class QuitarAutorRecursoCommand
{
    public int RecursoBibliograficoId { get; init; }

    public int AutorId { get; init; }
}