using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IIngredientRepository
{
    Task<List<Ingredient>> GetAllAsync(CancellationToken ct = default);
    Task<List<Ingredient>> GetLowStockAsync(CancellationToken ct = default);
    Task<Ingredient?> GetByIdAsync(int id, CancellationToken ct = default);
    void Add(Ingredient ingredient);
}
