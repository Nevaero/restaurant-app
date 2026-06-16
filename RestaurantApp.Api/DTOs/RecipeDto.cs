namespace RestaurantApp.Api.DTOs;

public record AllergenDto(int Id, string Name);

/// <summary>Per-serving nutritional values.</summary>
public record NutritionDto(int Calories, decimal Protein, decimal Carbohydrates, decimal Fat, decimal Sugars)
{
    public static readonly NutritionDto Empty = new(0, 0, 0, 0, 0);
}

public record RecipeIngredientDto(int IngredientId, string IngredientName, string Unit, decimal Quantity);

public record RecipeDto(
    int Id,
    string Name,
    string Instructions,
    int Servings,
    NutritionDto Nutrition,
    IReadOnlyList<RecipeIngredientDto> Ingredients,
    IReadOnlyList<string> Allergens);

/// <summary>Row for the recipe list view (also feeds the menu editor's import dropdown).</summary>
public record RecipeSummaryDto(
    int Id,
    string Name,
    int Servings,
    int IngredientCount,
    NutritionDto Nutrition,
    IReadOnlyList<string> Allergens);

public record RecipeIngredientInput(int IngredientId, decimal Quantity);

public record CreateRecipeRequest(
    string Name,
    string Instructions,
    int Servings,
    NutritionDto Nutrition,
    IReadOnlyList<RecipeIngredientInput> Ingredients);

public record UpdateRecipeRequest(
    string Name,
    string Instructions,
    int Servings,
    NutritionDto Nutrition,
    IReadOnlyList<RecipeIngredientInput> Ingredients);
