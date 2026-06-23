using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Prestamos;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Prestamos;

public sealed class ReservaTemporalRepository
    : BaseRepository<ReservaTemporal>, IReservaTemporalRepository
{
    public ReservaTemporalRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Busca si un ejemplar tiene una reserva activa vigente.
    public async Task<ReservaTemporal?> ObtenerActivaPorEjemplarAsync(
        int ejemplarId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(
                reserva =>
                    reserva.Activo &&
                    reserva.EjemplarId == ejemplarId &&
                    reserva.Estado == EstadoReserva.Activa,
                cancellationToken);
    }

    // Obtiene reservas activas que ya expiraron para liberarlas posteriormente.
    public async Task<IReadOnlyList<ReservaTemporal>> ObtenerVencidasAsync(
        DateTime fechaActual,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(reserva =>
                reserva.Activo &&
                reserva.Estado == EstadoReserva.Activa &&
                reserva.FechaExpiracion <= fechaActual)
            .OrderBy(reserva => reserva.FechaExpiracion)
            .ToListAsync(cancellationToken);
    }
}