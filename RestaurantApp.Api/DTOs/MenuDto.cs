namespace RestaurantApp.Api.DTOs;

public record MenuItemDto(int Id, string Dish, int IngredientId, string IngredientName, decimal QuantityRequired);

public record MenuDto(int Id, string Name, DateOnly Date, bool IsActive, IReadOnlyList<MenuItemDto> Items);

public record MenuItemInput(string Dish, int IngredientId, decimal QuantityRequired);

public record CreateMenuRequest(string Name, DateOnly Date, bool IsActive, IReadOnlyList<MenuItemInput> Items);

public record UpdateMenuRequest(string Name, DateOnly Date, bool IsActive, IReadOnlyList<MenuItemInput> Items);

public record ServeMenuRequest(int Servings = 1);

public record LowStockWarningDto(int IngredientId, string Name, decimal RemainingStock, decimal Threshold, string Unit);

public record ServeMenuResponse(bool Success, string? Error, IReadOnlyList<LowStockWarningDto> Warnings);
