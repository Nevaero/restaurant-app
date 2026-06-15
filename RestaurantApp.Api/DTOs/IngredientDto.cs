namespace RestaurantApp.Api.DTOs;

public record IngredientDto(
    int Id,
    string Name,
    decimal Quantity,
    string Unit,
    decimal LowStockThreshold,
    IReadOnlyList<AllergenDto> Allergens,
    bool IsLow);

public record CreateIngredientRequest(
    string Name,
    decimal Quantity,
    string Unit,
    decimal LowStockThreshold,
    IReadOnlyList<int> AllergenIds);

public record UpdateIngredientRequest(
    string Name,
    decimal Quantity,
    string Unit,
    decimal LowStockThreshold,
    IReadOnlyList<int> AllergenIds);
