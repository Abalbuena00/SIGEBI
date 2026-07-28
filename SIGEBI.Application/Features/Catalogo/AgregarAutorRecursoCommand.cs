namespace SIGEBI.Application.Features.Catalogo;

public sealed class AgregarAutorRecursoCommand
{
    public int RecursoBibliograficoId { get; init; }

    public int AutorId { get; init; }

    public int UsuarioResponsableId { get; init; }
}
