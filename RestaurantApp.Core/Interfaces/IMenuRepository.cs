using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IMenuRepository
{
    Task<List<Menu>> GetAllAsync(CancellationToken ct = default);
    Task<List<Menu>> GetActiveByDateAsync(DateOnly date, CancellationToken ct = default);
    Task<Menu?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Loads a menu together with its items and their ingredients.</summary>
    Task<Menu?> GetWithItemsAsync(int id, CancellationToken ct = default);

    void Add(Menu menu);
    void Remove(Menu menu);
}
