namespace RestaurantApp.Api.DTOs;

public record MenuRecipeDto(int RecipeId, string RecipeName, int Day);

public record MenuDto(
    int Id,
    string Name,
    DateOnly WeekStart,
    string Content,
    string NutritionalInfo,
    IReadOnlyList<MenuRecipeDto> Recipes,
    IReadOnlyList<string> Allergens);

/// <summary>Lightweight row for the menu list.</summary>
public record MenuSummaryDto(int Id, string Name, DateOnly WeekStart);

public record CreateMenuRequest(string Name, DateOnly WeekStart);

public record MenuRecipeInput(int RecipeId, int Day);

public record UpdateMenuRequest(
    string Name,
    DateOnly WeekStart,
    string Content,
    string NutritionalInfo,
    IReadOnlyList<MenuRecipeInput> Recipes);
