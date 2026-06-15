using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Repositories;

public class RecipeRepository(AppDbContext db) : IRecipeRepository
{
    private IQueryable<Recipe> WithGraph() =>
        db.Recipes
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .ThenInclude(i => i.Allergens);

    public Task<List<Recipe>> SearchAsync(string? term, CancellationToken ct = default)
    {
        var query = WithGraph();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.Trim();
            query = query.Where(r =>
                EF.Functions.Like(r.Name, $"%{t}%") ||
                EF.Functions.Like(r.Instructions, $"%{t}%") ||
                r.Ingredients.Any(ri => EF.Functions.Like(ri.Ingredient.Name, $"%{t}%")));
        }

        return query.OrderBy(r => r.Name).ToListAsync(ct);
    }

    public Task<Recipe?> GetByIdAsync(int id, CancellationToken ct = default) =>
        WithGraph().FirstOrDefaultAsync(r => r.Id == id, ct);

    public void Add(Recipe recipe) => db.Recipes.Add(recipe);

    public void Remove(Recipe recipe) => db.Recipes.Remove(recipe);
}
