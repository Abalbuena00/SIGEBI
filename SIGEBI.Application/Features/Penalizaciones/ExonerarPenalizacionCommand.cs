namespace SIGEBI.Application.Features.Penalizaciones;

public sealed class ExonerarPenalizacionCommand
{
    public int PenalizacionId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public string Motivo { get; init; } = string.Empty;
}
