namespace RestaurantApp.Api.DTOs;

public record MenuDayDto(
    int Day,
    string Dish,
    int? RecipeId,
    NutritionDto Nutrition,
    IReadOnlyList<string> Allergens);

public record MenuDto(
    int Id,
    string Name,
    DateOnly WeekStart,
    string Content,
    IReadOnlyList<MenuDayDto> Days,
    IReadOnlyList<string> Allergens);

/// <summary>Lightweight row for the menu list.</summary>
public record MenuSummaryDto(int Id, string Name, DateOnly WeekStart);

public record CreateMenuRequest(string Name, DateOnly WeekStart);

public record MenuDayInput(int Day, string Dish, int? RecipeId, NutritionDto Nutrition);

public record UpdateMenuRequest(
    string Name,
    DateOnly WeekStart,
    string Content,
    IReadOnlyList<MenuDayInput> Days);
