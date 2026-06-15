using RestaurantApp.Api.DTOs;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Services;

namespace RestaurantApp.Api;

/// <summary>Maps domain entities to response DTOs.</summary>
public static class Mapping
{
    public static MenuDto ToDto(this Menu menu) =>
        new(menu.Id, menu.Name, menu.WeekStart, menu.Content, menu.NutritionalInfo);

    public static MenuSummaryDto ToSummaryDto(this Menu menu) =>
        new(menu.Id, menu.Name, menu.WeekStart);

    public static IngredientDto ToDto(this Ingredient ingredient) =>
        new(
            ingredient.Id,
            ingredient.Name,
            ingredient.Quantity,
            ingredient.Unit,
            ingredient.LowStockThreshold,
            ingredient.Allergens,
            ingredient.Quantity < ingredient.LowStockThreshold);

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
