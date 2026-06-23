using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Auditoria;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Repositories.Auditoria;

public sealed class RegistroAuditoriaRepository : IRegistroAuditoriaRepository
{
    private readonly SigebiDbContext _context;

    public RegistroAuditoriaRepository(SigebiDbContext context)
    {
        _context = context;
    }

    // Agrega un registro de auditoría. Estos registros no deben modificarse después.
    public async Task AgregarAsync(
        RegistroAuditoria registro,
        CancellationToken cancellationToken = default)
    {
        await _context.RegistrosAuditoria.AddAsync(registro, cancellationToken);
    }

    // Consulta auditoría usando filtros opcionales.
    public async Task<IReadOnlyList<RegistroAuditoria>> ConsultarAsync(
        int? usuarioId,
        string? modulo,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken = default)
    {
        var query = _context.RegistrosAuditoria
            .AsNoTracking()
            .AsQueryable();

        if (usuarioId.HasValue)
            query = query.Where(registro => registro.UsuarioId == usuarioId.Value);

        if (!string.IsNullOrWhiteSpace(modulo))
        {
            var moduloNormalizado = modulo.Trim();

            query = query.Where(registro =>
                registro.Modulo == moduloNormalizado);
        }

        if (fechaDesde.HasValue)
            query = query.Where(registro => registro.FechaRegistro >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(registro => registro.FechaRegistro <= fechaHasta.Value);

        return await query
            .OrderByDescending(registro => registro.FechaRegistro)
            .ToListAsync(cancellationToken);
    }
}