namespace RestaurantApp.Api.DTOs;

public record AllergenDto(int Id, string Name);

public record RecipeIngredientDto(int IngredientId, string IngredientName, string Unit, decimal Quantity);

public record RecipeDto(
    int Id,
    string Name,
    string Instructions,
    int Servings,
    IReadOnlyList<RecipeIngredientDto> Ingredients,
    IReadOnlyList<string> Allergens);

/// <summary>Row for the recipe list view.</summary>
public record RecipeSummaryDto(
    int Id,
    string Name,
    int Servings,
    int IngredientCount,
    IReadOnlyList<string> Allergens);

public record RecipeIngredientInput(int IngredientId, decimal Quantity);

public record CreateRecipeRequest(
    string Name,
    string Instructions,
    int Servings,
    IReadOnlyList<RecipeIngredientInput> Ingredients);

public record UpdateRecipeRequest(
    string Name,
    string Instructions,
    int Servings,
    IReadOnlyList<RecipeIngredientInput> Ingredients);
