using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IMenuRepository
{
    Task<List<Menu>> GetAllAsync(CancellationToken ct = default);
    Task<Menu?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Loads a menu with its days, their source recipes, ingredients and allergens (for editing and PDF export).</summary>
    Task<Menu?> GetWithDaysAsync(int id, CancellationToken ct = default);

    void Add(Menu menu);
    void Remove(Menu menu);
}
