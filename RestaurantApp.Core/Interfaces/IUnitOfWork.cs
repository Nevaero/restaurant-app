namespace RestaurantApp.Core.Interfaces;

/// <summary>
/// Coordinates persistence and explicit transactions across repositories.
/// Repositories stage changes in the change tracker; this commits them.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct = default);
}

/// <summary>A database transaction scope that must be committed explicitly.</summary>
public interface IAppTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
