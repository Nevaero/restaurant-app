using FluentAssertions;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Services;
using Xunit;

namespace RestaurantApp.Tests;

public class AllergenSummaryTests
{
    private static Ingredient Ingredient(string name, params string[] allergens) => new()
    {
        Name = name,
        Allergens = allergens.Select(a => new Allergen { Name = a }).ToList(),
    };

    private static Recipe RecipeWith(params Ingredient[] ingredients) => new()
    {
        Name = "R",
        Ingredients = ingredients.Select(i => new RecipeIngredient { Ingredient = i }).ToList(),
    };

    [Fact]
    public void ForRecipe_ReturnsDistinctSortedAllergens()
    {
        var recipe = RecipeWith(
            Ingredient("Pasta", "Gluten"),
            Ingredient("Pesto", "Milk", "Nuts"),
            Ingredient("Parmesan", "Milk")); // Milk duplicated

        AllergenSummary.ForRecipe(recipe).Should().Equal("Gluten", "Milk", "Nuts");
    }

    [Fact]
    public void ForRecipe_IsEmpty_WhenNoAllergens()
    {
        var recipe = RecipeWith(Ingredient("Tomatoes"), Ingredient("Water"));
        AllergenSummary.ForRecipe(recipe).Should().BeEmpty();
    }

    [Fact]
    public void ForMenu_UnionsAllergensAcrossAllRecipes()
    {
        var menu = new Menu
        {
            Name = "Week",
            MenuRecipes =
            [
                new MenuRecipe { Recipe = RecipeWith(Ingredient("Pasta", "Gluten")) },
                new MenuRecipe { Recipe = RecipeWith(Ingredient("Shrimp", "Crustaceans"), Ingredient("Pasta", "Gluten")) },
            ],
        };

        AllergenSummary.ForMenu(menu).Should().Equal("Crustaceans", "Gluten");
    }
}
