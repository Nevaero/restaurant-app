using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IIngredientRepository
{
    /// <summary>All ingredients with their allergens.</summary>
    Task<List<Ingredient>> GetAllAsync(CancellationToken ct = default);
    Task<List<Ingredient>> GetLowStockAsync(CancellationToken ct = default);

    /// <summary>An ingredient with its allergens.</summary>
    Task<Ingredient?> GetByIdAsync(int id, CancellationToken ct = default);

    void Add(Ingredient ingredient);
    void Remove(Ingredient ingredient);
}
