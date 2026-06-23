using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SigebiDbContext _context;

    public UnitOfWork(SigebiDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // Confirma en la base de datos todos los cambios pendientes del DbContext.
        return await _context.SaveChangesAsync(cancellationToken);
    }
}