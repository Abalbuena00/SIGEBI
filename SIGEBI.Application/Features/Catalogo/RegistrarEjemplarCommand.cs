namespace SIGEBI.Application.Features.Catalogo;

public sealed class RegistrarEjemplarCommand
{
    public int RecursoBibliograficoId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public string CodigoInterno { get; init; } = string.Empty;

    public string? EstadoFisico { get; init; }
}