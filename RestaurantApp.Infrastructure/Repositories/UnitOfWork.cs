using Microsoft.EntityFrameworkCore.Storage;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);

    public async Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct = default) =>
        new EfTransaction(await db.Database.BeginTransactionAsync(ct));

    private sealed class EfTransaction(IDbContextTransaction transaction) : IAppTransaction
    {
        public Task CommitAsync(CancellationToken ct = default) => transaction.CommitAsync(ct);
        public Task RollbackAsync(CancellationToken ct = default) => transaction.RollbackAsync(ct);
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
