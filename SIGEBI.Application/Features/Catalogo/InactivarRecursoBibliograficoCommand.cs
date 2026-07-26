namespace SIGEBI.Application.Features.Catalogo;

public sealed class InactivarRecursoBibliograficoCommand
{
    public int RecursoBibliograficoId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public string? Motivo { get; init; }
}
