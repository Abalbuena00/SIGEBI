using SIGEBI.Domain.Entities.Seguridad;

namespace SIGEBI.Domain.Repository;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> ObtenerPorCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Usuario>> ObtenerUsuariosActivosAsync(
        CancellationToken cancellationToken = default);
}