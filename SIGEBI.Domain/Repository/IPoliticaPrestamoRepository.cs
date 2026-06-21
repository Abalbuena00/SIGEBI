using SIGEBI.Domain.Entities.Configuracion;
using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Repository;

public interface IPoliticaPrestamoRepository : IBaseRepository<PoliticaPrestamo>
{
    Task<PoliticaPrestamo?> ObtenerPorTipoMiembroAsync(
        TipoMiembro tipoMiembro,
        CancellationToken cancellationToken = default);
}