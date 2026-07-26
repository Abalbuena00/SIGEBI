using SIGEBI.Domain.Entities.Auditoria;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Repository;

public interface IRegistroAuditoriaRepository
{
    Task AgregarAsync(
        RegistroAuditoria registro,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<RegistroAuditoria> Items, int TotalItems)> ConsultarAsync(
        int? usuarioId,
        string? modulo,
        ResultadoAuditoria? resultado,
        string? entidadAfectada,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
