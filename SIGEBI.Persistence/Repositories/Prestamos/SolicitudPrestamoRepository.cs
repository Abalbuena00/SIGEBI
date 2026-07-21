using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Prestamos;

public sealed class SolicitudPrestamoRepository
    : BaseRepository<SolicitudPrestamo>, ISolicitudPrestamoRepository
{
    public SolicitudPrestamoRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Obtiene las solicitudes que todavía esperan revisión del personal bibliotecario.
    public async Task<IReadOnlyList<SolicitudPrestamo>> ObtenerPendientesAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(solicitud =>
                solicitud.Activo &&
                solicitud.Estado == EstadoSolicitudPrestamo.Pendiente)
            .OrderBy(solicitud => solicitud.FechaSolicitud)
            .ToListAsync(cancellationToken);
    }

    // Obtiene el historial de solicitudes realizadas por un usuario.
    public async Task<IReadOnlyList<SolicitudPrestamo>> ObtenerPorUsuarioAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(solicitud =>
                solicitud.Activo &&
                solicitud.UsuarioId == usuarioId)
            .OrderByDescending(solicitud => solicitud.FechaSolicitud)
            .ToListAsync(cancellationToken);
    }

    // Obtiene las solicitudes que han vencido y aún no han sido revisadas por el personal bibliotecario.
    public async Task<IReadOnlyList<SolicitudPrestamo>> ObtenerVenciblesAsync(
    DateTime fechaActual,
    CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(solicitud =>
                solicitud.Activo &&
                solicitud.Estado == EstadoSolicitudPrestamo.Pendiente &&
                solicitud.FechaExpiracionSolicitud <= fechaActual)
            .ToListAsync(cancellationToken);
    }
}