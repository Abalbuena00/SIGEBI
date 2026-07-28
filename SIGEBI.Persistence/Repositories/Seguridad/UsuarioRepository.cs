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

    public async Task<Usuario?> ObtenerConRolesPorIdAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(usuario => usuario.Roles)
                .ThenInclude(usuarioRol => usuarioRol.Rol)
            .FirstOrDefaultAsync(
                usuario => usuario.Id == usuarioId && usuario.Activo,
                cancellationToken);
    }

    public void RemoverRol(UsuarioRol usuarioRol)
    {
        Context.Set<UsuarioRol>().Remove(usuarioRol);
    }

    // Busca usuarios con filtros de texto, estado y rol, y devuelve resultados paginados.
    public async Task<(IReadOnlyList<Usuario> Items, int TotalCount)> BuscarAsync(
        string? textoBusqueda,
        EstadoUsuario? estado,
        int? rolId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Roles)
                .ThenInclude(usuarioRol => usuarioRol.Rol)
            .Where(usuario => usuario.Activo)
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

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(usuario => usuario.NombreCompleto)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
