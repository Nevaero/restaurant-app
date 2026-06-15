namespace RestaurantApp.Web.Models;

// Client-side mirrors of the API contract. A standalone WebAssembly client keeps its
// own DTOs rather than referencing the server project.

public record MenuItemDto(int Id, string Dish, int IngredientId, string IngredientName, decimal QuantityRequired);

public record MenuDto(int Id, string Name, DateOnly Date, bool IsActive, IReadOnlyList<MenuItemDto> Items);

public record MenuItemInput(string Dish, int IngredientId, decimal QuantityRequired);

public record CreateMenuRequest(string Name, DateOnly Date, bool IsActive, IReadOnlyList<MenuItemInput> Items);

public record UpdateMenuRequest(string Name, DateOnly Date, bool IsActive, IReadOnlyList<MenuItemInput> Items);

public record ServeMenuRequest(int Servings);

public record LowStockWarningDto(int IngredientId, string Name, decimal RemainingStock, decimal Threshold, string Unit);

public record ServeMenuResponse(bool Success, string? Error, IReadOnlyList<LowStockWarningDto> Warnings);

public record IngredientDto(int Id, string Name, decimal StockQuantity, decimal LowStockThreshold, string Unit, bool IsLow);

public record CreateIngredientRequest(string Name, decimal StockQuantity, decimal LowStockThreshold, string Unit);

public record UpdateStockRequest(decimal StockQuantity);

public record EmployeeDto(int Id, string FirstName, string LastName, string Role)
{
    public string FullName => $"{FirstName} {LastName}";
}

public record ShiftDto(int Id, int EmployeeId, string EmployeeName, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string? Notes);

public record CreateShiftRequest(int EmployeeId, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string? Notes);
