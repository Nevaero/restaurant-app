using RestaurantApp.Api.DTOs;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Services;

namespace RestaurantApp.Api;

/// <summary>Maps domain entities to response DTOs.</summary>
public static class Mapping
{
    public static AllergenDto ToDto(this Allergen allergen) => new(allergen.Id, allergen.Name);

    public static MenuSummaryDto ToSummaryDto(this Menu menu) =>
        new(menu.Id, menu.Name, menu.WeekStart);

    public static MenuDto ToDto(this Menu menu) =>
        new(
            menu.Id,
            menu.Name,
            menu.WeekStart,
            menu.Content,
            menu.NutritionalInfo,
            menu.MenuRecipes
                .OrderBy(mr => mr.Day)
                .Select(mr => new MenuRecipeDto(mr.RecipeId, mr.Recipe?.Name ?? string.Empty, mr.Day))
                .ToList(),
            AllergenSummary.ForMenu(menu));

    public static IngredientDto ToDto(this Ingredient ingredient) =>
        new(
            ingredient.Id,
            ingredient.Name,
            ingredient.Quantity,
            ingredient.Unit,
            ingredient.LowStockThreshold,
            ingredient.Allergens.OrderBy(a => a.Name).Select(a => a.ToDto()).ToList(),
            ingredient.Quantity < ingredient.LowStockThreshold);

    public static RecipeIngredientDto ToDto(this RecipeIngredient ri) =>
        new(ri.IngredientId, ri.Ingredient?.Name ?? string.Empty, ri.Ingredient?.Unit ?? string.Empty, ri.Quantity);

    public static RecipeDto ToDto(this Recipe recipe) =>
        new(
            recipe.Id,
            recipe.Name,
            recipe.Instructions,
            recipe.Servings,
            recipe.Ingredients.Select(ri => ri.ToDto()).ToList(),
            AllergenSummary.ForRecipe(recipe));

    public static RecipeSummaryDto ToSummaryDto(this Recipe recipe) =>
        new(recipe.Id, recipe.Name, recipe.Servings, recipe.Ingredients.Count, AllergenSummary.ForRecipe(recipe));

    public static EmployeeDto ToDto(this Employee employee) =>
        new(employee.Id, employee.FirstName, employee.LastName, employee.Role, employee.HourlyRate);

    public static ShiftDto ToDto(this Shift shift) =>
        new(
            shift.Id,
            shift.EmployeeId,
            shift.Employee?.FullName ?? string.Empty,
            shift.Employee?.Role ?? string.Empty,
            shift.Date,
            shift.StartTime,
            shift.EndTime,
            shift.Notes);

    public static EmployeeWeekSummaryDto ToDto(this EmployeeWeekSummary summary) =>
        new(summary.EmployeeId, summary.Name, summary.Role, summary.TotalHours, summary.LaborCost, summary.IsOvertime);
}
