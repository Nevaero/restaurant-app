using FluentAssertions;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Services;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Repositories;
using Xunit;

namespace RestaurantApp.Tests;

public class StockDeductionServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    private StockDeductionService CreateService(AppDbContext db) =>
        new(new MenuRepository(db), new UnitOfWork(db));

    /// <summary>Seeds one menu whose single dish uses two ingredients, and returns its id.</summary>
    private async Task<int> SeedMenuAsync(
        decimal tomatoStock = 10m,
        decimal tomatoThreshold = 2m,
        decimal pastaStock = 10m,
        decimal pastaThreshold = 2m,
        decimal tomatoPerServing = 1m,
        decimal pastaPerServing = 1m)
    {
        await using var db = _factory.CreateContext();

        var tomatoes = new Ingredient { Name = "Tomatoes", StockQuantity = tomatoStock, LowStockThreshold = tomatoThreshold, Unit = "kg" };
        var pasta = new Ingredient { Name = "Pasta", StockQuantity = pastaStock, LowStockThreshold = pastaThreshold, Unit = "kg" };

        var menu = new Menu
        {
            Name = "Test Menu",
            Date = DateOnly.FromDateTime(DateTime.Today),
            IsActive = true,
            MenuItems =
            [
                new MenuItem { Dish = "Pasta", Ingredient = pasta, QuantityRequired = pastaPerServing },
                new MenuItem { Dish = "Pasta", Ingredient = tomatoes, QuantityRequired = tomatoPerServing },
            ],
        };

        db.Menus.Add(menu);
        await db.SaveChangesAsync();
        return menu.Id;
    }

    [Fact]
    public async Task ServeMenu_ReducesStock_ForEachIngredient()
    {
        var menuId = await SeedMenuAsync(tomatoStock: 10m, pastaStock: 10m, tomatoPerServing: 1m, pastaPerServing: 2m);

        await using (var db = _factory.CreateContext())
        {
            var result = await CreateService(db).ServeMenuAsync(menuId, servings: 3);
            result.Success.Should().BeTrue();
            result.Error.Should().BeNull();
        }

        await using (var db = _factory.CreateContext())
        {
            var tomatoes = db.Ingredients.Single(i => i.Name == "Tomatoes");
            var pasta = db.Ingredients.Single(i => i.Name == "Pasta");
            tomatoes.StockQuantity.Should().Be(7m);  // 10 - (1 * 3)
            pasta.StockQuantity.Should().Be(4m);      // 10 - (2 * 3)
        }
    }

    [Fact]
    public async Task ServeMenu_ReturnsLowStockWarning_WhenStockFallsBelowThreshold()
    {
        // Pasta starts at 5, threshold 4, serving 2 → ends at 3 (below threshold).
        var menuId = await SeedMenuAsync(
            pastaStock: 5m, pastaThreshold: 4m, pastaPerServing: 2m,
            tomatoStock: 20m, tomatoThreshold: 2m, tomatoPerServing: 1m);

        await using var db = _factory.CreateContext();
        var result = await CreateService(db).ServeMenuAsync(menuId, servings: 1);

        result.Success.Should().BeTrue();
        result.Warnings.Should().ContainSingle()
            .Which.Name.Should().Be("Pasta");
        result.Warnings[0].RemainingStock.Should().Be(3m);
        result.Warnings[0].Threshold.Should().Be(4m);
    }

    [Fact]
    public async Task ServeMenu_RollsBackAllDeductions_WhenOneIngredientIsInsufficient()
    {
        // Pasta has plenty (10), tomatoes are short (1) for a 5-serving order.
        var menuId = await SeedMenuAsync(
            pastaStock: 10m, pastaPerServing: 1m,
            tomatoStock: 1m, tomatoPerServing: 1m);

        await using (var db = _factory.CreateContext())
        {
            var result = await CreateService(db).ServeMenuAsync(menuId, servings: 5);
            result.Success.Should().BeFalse();
            result.Error.Should().Contain("Tomatoes");
        }

        // No deductions should have been applied to ANY ingredient.
        await using (var db = _factory.CreateContext())
        {
            db.Ingredients.Single(i => i.Name == "Pasta").StockQuantity.Should().Be(10m);
            db.Ingredients.Single(i => i.Name == "Tomatoes").StockQuantity.Should().Be(1m);
        }
    }

    [Fact]
    public async Task ServeMenu_ReturnsFailure_WhenMenuDoesNotExist()
    {
        await using var db = _factory.CreateContext();
        var result = await CreateService(db).ServeMenuAsync(menuId: 999, servings: 1);

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task ServeMenu_ReturnsFailure_WhenServingsNotPositive(int servings)
    {
        var menuId = await SeedMenuAsync();

        await using var db = _factory.CreateContext();
        var result = await CreateService(db).ServeMenuAsync(menuId, servings);

        result.Success.Should().BeFalse();
        result.Warnings.Should().BeEmpty();
    }
}
