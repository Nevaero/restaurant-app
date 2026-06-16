using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Services;

/// <summary>Aggregates allergens upward: from ingredients to recipes to menus.</summary>
public static class AllergenSummary
{
    public static IReadOnlyList<string> ForRecipe(Recipe recipe) =>
        recipe.Ingredients
            .SelectMany(ri => ri.Ingredient?.Allergens ?? [])
            .Select(a => a.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToList();

    public static IReadOnlyList<string> ForMenu(Menu menu) =>
        menu.Days
            .Where(d => d.Recipe is not null)
            .SelectMany(d => ForRecipe(d.Recipe!))
            .Distinct()
            .OrderBy(name => name)
            .ToList();
}
