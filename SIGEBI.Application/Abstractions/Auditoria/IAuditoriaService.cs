using SIGEBI.Domain.Enums;

namespace SIGEBI.Application.Abstractions.Auditoria;

public interface IAuditoriaService
{
    Task RegistrarAsync(
        int? usuarioId,
        string modulo,
        string accion,
        ResultadoAuditoria resultado,
        string? entidadAfectada = null,
        int? entidadAfectadaId = null,
        string? detalle = null,
        string? direccionIp = null,
        string? origen = null,
        CancellationToken cancellationToken = default);
}