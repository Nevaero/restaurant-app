namespace RestaurantApp.Web.Models;

// Client-side mirrors of the API contract. A standalone WebAssembly client keeps its
// own DTOs rather than referencing the server project.

// Menus
public record MenuSummaryDto(int Id, string Name, DateOnly WeekStart);

public record MenuDto(int Id, string Name, DateOnly WeekStart, string Content, string NutritionalInfo);

public record CreateMenuRequest(string Name, DateOnly WeekStart);

public record UpdateMenuRequest(string Name, DateOnly WeekStart, string Content, string NutritionalInfo);

// Inventory
public record IngredientDto(int Id, string Name, decimal Quantity, string Unit, decimal LowStockThreshold, string Allergens, bool IsLow);

public record CreateIngredientRequest(string Name, decimal Quantity, string Unit, decimal LowStockThreshold, string Allergens);

public record UpdateIngredientRequest(string Name, decimal Quantity, string Unit, decimal LowStockThreshold, string Allergens);

// Staff & planning
public record EmployeeDto(int Id, string FirstName, string LastName, string Role, decimal HourlyRate)
{
    public string FullName => $"{FirstName} {LastName}";
}

public record CreateEmployeeRequest(string FirstName, string LastName, string Role, decimal HourlyRate);

public record ShiftDto(int Id, int EmployeeId, string EmployeeName, string Role, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string? Notes);

public record CreateShiftRequest(int EmployeeId, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string? Notes);

public record EmployeeWeekSummaryDto(int EmployeeId, string Name, string Role, decimal TotalHours, decimal LaborCost, bool IsOvertime);

public record WeekScheduleDto(DateOnly WeekStart, IReadOnlyList<ShiftDto> Shifts, IReadOnlyList<EmployeeWeekSummaryDto> Summaries);
