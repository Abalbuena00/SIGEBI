using SIGEBI.Domain.Enums;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class CambiarEstadoUsuarioCommand
{
    public int UsuarioId { get; init; }

    public int UsuarioResponsableId { get; init; }

    public EstadoUsuario NuevoEstado { get; init; }
}