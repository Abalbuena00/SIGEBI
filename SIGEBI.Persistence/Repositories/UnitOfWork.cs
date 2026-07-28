using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Exceptions;
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

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        return new EfCoreUnitOfWorkTransaction(transaction);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyException(
                "La información fue modificada por otro proceso. Intente nuevamente.",
                exception);
        }
    }
}
