using SIGEBI.Domain.Enums;

namespace SIGEBI.Application.Features.Seguridad;

public sealed class ConsultarUsuariosQuery
{
    public string? TextoBusqueda { get; init; }

    public EstadoUsuario? Estado { get; init; }

    public int? RolId { get; init; }
}