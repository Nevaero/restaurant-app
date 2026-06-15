using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Repositories;

public class IngredientRepository(AppDbContext db) : IIngredientRepository
{
    public Task<List<Ingredient>> GetAllAsync(CancellationToken ct = default) =>
        db.Ingredients.OrderBy(i => i.Name).ToListAsync(ct);

    public Task<List<Ingredient>> GetLowStockAsync(CancellationToken ct = default) =>
        db.Ingredients
            .Where(i => i.StockQuantity < i.LowStockThreshold)
            .OrderBy(i => i.Name)
            .ToListAsync(ct);

    public Task<Ingredient?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Ingredients.FirstOrDefaultAsync(i => i.Id == id, ct);

    public void Add(Ingredient ingredient) => db.Ingredients.Add(ingredient);
}
