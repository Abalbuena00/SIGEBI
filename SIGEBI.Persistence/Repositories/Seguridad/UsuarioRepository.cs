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

    // Busca usuarios según criterios de búsqueda, estado y rol.
    public async Task<IReadOnlyList<Usuario>> BuscarAsync(
    string? textoBusqueda,
    EstadoUsuario? estado,
    int? rolId,
    CancellationToken cancellationToken = default)
    {
        var query = Context.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Roles)
                .ThenInclude(usuarioRol => usuarioRol.Rol)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(textoBusqueda))
        {
            string filtro = $"%{textoBusqueda.Trim()}%";

            query = query.Where(usuario =>
                EF.Functions.Like(usuario.NombreCompleto, filtro) ||
                EF.Functions.Like(usuario.Correo, filtro) ||
                usuario.Matricula != null && EF.Functions.Like(usuario.Matricula, filtro) ||
                usuario.NumeroEmpleado != null && EF.Functions.Like(usuario.NumeroEmpleado, filtro));
        }

        if (estado.HasValue)
        {
            query = query.Where(usuario =>
                usuario.Estado == estado.Value);
        }

        if (rolId.HasValue)
        {
            query = query.Where(usuario =>
                usuario.Roles.Any(usuarioRol =>
                    usuarioRol.RolId == rolId.Value));
        }

        return await query
            .OrderBy(usuario => usuario.NombreCompleto)
            .ToListAsync(cancellationToken);
    }
}
