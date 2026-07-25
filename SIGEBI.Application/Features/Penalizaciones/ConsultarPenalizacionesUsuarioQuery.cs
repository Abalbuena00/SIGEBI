using SIGEBI.Domain.Enums;

namespace SIGEBI.Application.Features.Penalizaciones;

public sealed class ConsultarPenalizacionesUsuarioQuery
{
    public int UsuarioId { get; init; }

    public EstadoPenalizacion? Estado { get; init; }
}
