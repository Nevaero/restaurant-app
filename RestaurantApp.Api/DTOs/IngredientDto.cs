namespace RestaurantApp.Api.DTOs;

public record IngredientDto(
    int Id,
    string Name,
    decimal Quantity,
    string Unit,
    decimal LowStockThreshold,
    string Allergens,
    bool IsLow);

public record CreateIngredientRequest(
    string Name,
    decimal Quantity,
    string Unit,
    decimal LowStockThreshold,
    string Allergens);

public record UpdateIngredientRequest(
    string Name,
    decimal Quantity,
    string Unit,
    decimal LowStockThreshold,
    string Allergens);
