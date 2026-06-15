using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IRecipeRepository
{
    /// <summary>All recipes (with ingredients + allergens), optionally filtered by a search term.</summary>
    Task<List<Recipe>> SearchAsync(string? term, CancellationToken ct = default);

    /// <summary>A recipe with its ingredients and their allergens.</summary>
    Task<Recipe?> GetByIdAsync(int id, CancellationToken ct = default);

    void Add(Recipe recipe);
    void Remove(Recipe recipe);
}
