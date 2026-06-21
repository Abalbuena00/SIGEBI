using SIGEBI.Domain.Entities.Configuracion;

namespace SIGEBI.Domain.Repository;

public interface IParametroSistemaRepository : IBaseRepository<ParametroSistema>
{
    Task<ParametroSistema?> ObtenerPorClaveAsync(
        string clave,
        CancellationToken cancellationToken = default);
}