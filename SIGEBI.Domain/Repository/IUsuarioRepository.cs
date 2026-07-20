using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Domain.Enums;

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

    Task<IReadOnlyList<Usuario>> BuscarAsync(
    string? textoBusqueda,
    EstadoUsuario? estado,
    int? rolId,
    CancellationToken cancellationToken = default);
}