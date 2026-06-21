using SIGEBI.Domain.Entities.Auditoria;

namespace SIGEBI.Domain.Repository;

public interface IRegistroAuditoriaRepository
{
    Task AgregarAsync(
        RegistroAuditoria registro,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RegistroAuditoria>> ConsultarAsync(
        int? usuarioId,
        string? modulo,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken = default);
}