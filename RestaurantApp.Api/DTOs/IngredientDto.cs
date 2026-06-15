namespace RestaurantApp.Api.DTOs;

public record IngredientDto(
    int Id,
    string Name,
    decimal StockQuantity,
    decimal LowStockThreshold,
    string Unit,
    bool IsLow);

public record CreateIngredientRequest(
    string Name,
    decimal StockQuantity,
    decimal LowStockThreshold,
    string Unit);

public record UpdateStockRequest(decimal StockQuantity);
