using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Auditoria;
using SIGEBI.Domain.Enums;
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

    public async Task<(IReadOnlyList<RegistroAuditoria> Items, int TotalItems)> ConsultarAsync(
        int? usuarioId,
        string? modulo,
        ResultadoAuditoria? resultado,
        string? entidadAfectada,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.RegistrosAuditoria.AsNoTracking().AsQueryable();

        if (usuarioId.HasValue)
            query = query.Where(registro => registro.UsuarioId == usuarioId.Value);

        if (!string.IsNullOrWhiteSpace(modulo))
        {
            var moduloNormalizado = modulo.Trim();
            query = query.Where(registro => registro.Modulo == moduloNormalizado);
        }

        if (resultado.HasValue)
            query = query.Where(registro => registro.Resultado == resultado.Value);

        if (!string.IsNullOrWhiteSpace(entidadAfectada))
        {
            var entidadNormalizada = entidadAfectada.Trim();
            query = query.Where(registro => registro.EntidadAfectada == entidadNormalizada);
        }

        if (fechaDesde.HasValue)
            query = query.Where(registro => registro.FechaRegistro >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(registro => registro.FechaRegistro <= fechaHasta.Value);

        int totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(registro => registro.FechaRegistro)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }
}
