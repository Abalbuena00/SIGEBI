namespace SIGEBI.Application.Features.Catalogo;
using SIGEBI.Application.Abstractions.Auditoria;

public sealed class MarcarEjemplarFueraDeServicioCommand
{
    public int EjemplarId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public string Motivo { get; init; } = string.Empty;
}