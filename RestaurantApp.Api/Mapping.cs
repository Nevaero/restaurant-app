using RestaurantApp.Api.DTOs;
using RestaurantApp.Core.Entities;

namespace RestaurantApp.Api;

/// <summary>Maps domain entities to response DTOs.</summary>
public static class Mapping
{
    public static MenuDto ToDto(this Menu menu) => new(
        menu.Id,
        menu.Name,
        menu.Date,
        menu.IsActive,
        menu.MenuItems
            .Select(i => new MenuItemDto(
                i.Id,
                i.Dish,
                i.IngredientId,
                i.Ingredient?.Name ?? string.Empty,
                i.QuantityRequired))
            .ToList());

    public static IngredientDto ToDto(this Ingredient ingredient) => new(
        ingredient.Id,
        ingredient.Name,
        ingredient.StockQuantity,
        ingredient.LowStockThreshold,
        ingredient.Unit,
        ingredient.StockQuantity < ingredient.LowStockThreshold);

    public static ShiftDto ToDto(this Shift shift) => new(
        shift.Id,
        shift.EmployeeId,
        shift.Employee is null ? string.Empty : $"{shift.Employee.FirstName} {shift.Employee.LastName}",
        shift.Date,
        shift.StartTime,
        shift.EndTime,
        shift.Notes);
}
