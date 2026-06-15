using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Tests;

/// <summary>
/// Builds an AppDbContext backed by an in-memory SQLite database. Unlike the EF
/// in-memory provider, SQLite supports real transactions — required to exercise the
/// rollback behaviour of <c>StockDeductionService</c>.
/// </summary>
public sealed class TestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public TestDbContextFactory()
    {
        // Keep one connection open for the lifetime of the test so the in-memory
        // database is not discarded between context instances.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var db = CreateContext();
        db.Database.EnsureCreated();
    }

    public AppDbContext CreateContext() => new(_options);

    public void Dispose() => _connection.Dispose();
}
