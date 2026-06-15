using RestaurantApp.Core.Interfaces;

namespace RestaurantApp.Core.Services;

/// <summary>Raised when an ingredient drops below its low-stock threshold after a service.</summary>
public record LowStockWarning(
    int IngredientId,
    string Name,
    decimal RemainingStock,
    decimal Threshold,
    string Unit);

/// <summary>Outcome of serving a menu. On failure no stock is deducted.</summary>
public record StockDeductionResult(
    bool Success,
    string? Error,
    IReadOnlyList<LowStockWarning> Warnings)
{
    public static StockDeductionResult Failure(string error) =>
        new(false, error, []);

    public static StockDeductionResult Ok(IReadOnlyList<LowStockWarning> warnings) =>
        new(true, null, warnings);
}

/// <summary>
/// Deducts ingredient stock when a menu is served. All deductions happen inside a
/// single transaction: if any ingredient lacks sufficient stock, nothing is changed.
/// </summary>
public class StockDeductionService(IMenuRepository menus, IUnitOfWork unitOfWork)
{
    public async Task<StockDeductionResult> ServeMenuAsync(
        int menuId,
        int servings,
        CancellationToken ct = default)
    {
        if (servings < 1)
            return StockDeductionResult.Failure("Servings must be at least 1.");

        var menu = await menus.GetWithItemsAsync(menuId, ct);
        if (menu is null)
            return StockDeductionResult.Failure($"Menu {menuId} was not found.");

        // Aggregate the required quantity per ingredient — a menu may use the same
        // ingredient across several dishes.
        var required = menu.MenuItems
            .GroupBy(item => item.Ingredient)
            .Select(g => (Ingredient: g.Key, Total: g.Sum(i => i.QuantityRequired) * servings))
            .ToList();

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        // Validate everything before mutating anything so a shortfall rolls the
        // whole operation back without partial deductions.
        foreach (var (ingredient, total) in required)
        {
            if (ingredient.StockQuantity < total)
            {
                await transaction.RollbackAsync(ct);
                return StockDeductionResult.Failure(
                    $"Insufficient stock for '{ingredient.Name}': " +
                    $"need {total} {ingredient.Unit}, have {ingredient.StockQuantity} {ingredient.Unit}.");
            }
        }

        var warnings = new List<LowStockWarning>();
        foreach (var (ingredient, total) in required)
        {
            ingredient.StockQuantity -= total;
            if (ingredient.StockQuantity < ingredient.LowStockThreshold)
            {
                warnings.Add(new LowStockWarning(
                    ingredient.Id,
                    ingredient.Name,
                    ingredient.StockQuantity,
                    ingredient.LowStockThreshold,
                    ingredient.Unit));
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return StockDeductionResult.Ok(warnings);
    }
}
