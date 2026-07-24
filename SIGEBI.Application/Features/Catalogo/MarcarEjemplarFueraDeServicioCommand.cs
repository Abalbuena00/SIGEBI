namespace SIGEBI.Application.Features.Catalogo;

public sealed class MarcarEjemplarFueraDeServicioCommand
{
    public int EjemplarId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public string Motivo { get; init; } = string.Empty;
}