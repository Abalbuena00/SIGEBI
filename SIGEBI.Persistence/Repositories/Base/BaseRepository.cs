using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Repositories.Base;

public class BaseRepository<T> : IBaseRepository<T>
    where T : BaseEntity
{
    protected readonly SigebiDbContext Context;
    protected readonly DbSet<T> DbSet;

    public BaseRepository(SigebiDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    // Busca una entidad por su Id usando la llave primaria configurada en EF Core.
    public async Task<T?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(
            new object[] { id },
            cancellationToken);
    }

    // Obtiene todos los registros de la entidad sin seguimiento de cambios.
    // AsNoTracking mejora el rendimiento cuando solo se va a consultar información.
    public async Task<IReadOnlyList<T>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    // Agrega una nueva entidad al contexto.
    // El registro todavía no se guarda en la base de datos hasta ejecutar SaveChangesAsync.
    public async Task AgregarAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    // Marca una entidad existente como modificada.
    // EF Core actualizará sus valores cuando se confirme la unidad de trabajo.
    public void Actualizar(T entity)
    {
        DbSet.Update(entity);
    }

    // Marca una entidad para eliminación.
    // En entidades auditables podríamos usar eliminación lógica desde la capa de aplicación.
    public void Remover(T entity)
    {
        DbSet.Remove(entity);
    }
}