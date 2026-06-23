using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities.Seguridad;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.Base;

namespace SIGEBI.Persistence.Repositories.Seguridad;

public sealed class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(SigebiDbContext context)
        : base(context)
    {
    }

    // Busca un usuario por correo. Se normaliza el valor para mantener consistencia.
    public async Task<Usuario?> ObtenerPorCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default)
    {
        var correoNormalizado = correo.Trim().ToLower();

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                usuario => usuario.Correo == correoNormalizado,
                cancellationToken);
    }

    // Verifica si ya existe un usuario registrado con ese correo.
    public async Task<bool> ExisteCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default)
    {
        var correoNormalizado = correo.Trim().ToLower();

        return await DbSet
            .AnyAsync(
                usuario => usuario.Correo == correoNormalizado,
                cancellationToken);
    }

    // Obtiene solo usuarios activos y habilitados en el sistema.
    public async Task<IReadOnlyList<Usuario>> ObtenerUsuariosActivosAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(usuario =>
                usuario.Activo &&
                usuario.Estado == EstadoUsuario.Activo)
            .ToListAsync(cancellationToken);
    }
}
