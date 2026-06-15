using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IMenuRepository
{
    Task<List<Menu>> GetAllAsync(CancellationToken ct = default);
    Task<Menu?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Loads a menu with its recipes, their ingredients and allergens (for editing and PDF export).</summary>
    Task<Menu?> GetWithRecipesAsync(int id, CancellationToken ct = default);

    void Add(Menu menu);
    void Remove(Menu menu);
}
