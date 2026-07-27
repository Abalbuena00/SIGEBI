namespace SIGEBI.Application.Features.Seguridad;

public sealed class AsignarRolUsuarioCommand
{
    public int UsuarioId { get; init; }
    public int RolId { get; init; }
    public int UsuarioResponsableId { get; init; }
}
