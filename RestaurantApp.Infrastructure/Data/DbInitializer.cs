using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;

namespace RestaurantApp.Infrastructure.Data;

/// <summary>Seeds a small, realistic data set so the app is usable on first run.</summary>
public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (await db.Ingredients.AnyAsync(ct))
            return; // already seeded

        var tomatoes = new Ingredient { Name = "Tomatoes", StockQuantity = 25m, LowStockThreshold = 5m, Unit = "kg" };
        var pasta = new Ingredient { Name = "Pasta", StockQuantity = 40m, LowStockThreshold = 10m, Unit = "kg" };
        var beef = new Ingredient { Name = "Ground Beef", StockQuantity = 12m, LowStockThreshold = 4m, Unit = "kg" };
        var cheese = new Ingredient { Name = "Parmesan", StockQuantity = 6m, LowStockThreshold = 2m, Unit = "kg" };
        var lettuce = new Ingredient { Name = "Lettuce", StockQuantity = 8m, LowStockThreshold = 3m, Unit = "kg" };
        db.Ingredients.AddRange(tomatoes, pasta, beef, cheese, lettuce);

        var today = DateOnly.FromDateTime(DateTime.Today);

        var lunchMenu = new Menu
        {
            Name = "Lunch Menu #1",
            Date = today,
            IsActive = true,
            MenuItems =
            [
                new MenuItem { Dish = "Pasta Bolognese", Ingredient = pasta, QuantityRequired = 0.15m },
                new MenuItem { Dish = "Pasta Bolognese", Ingredient = beef, QuantityRequired = 0.12m },
                new MenuItem { Dish = "Pasta Bolognese", Ingredient = tomatoes, QuantityRequired = 0.10m },
                new MenuItem { Dish = "Pasta Bolognese", Ingredient = cheese, QuantityRequired = 0.02m },
            ],
        };

        var saladMenu = new Menu
        {
            Name = "Lunch Menu #2",
            Date = today,
            IsActive = true,
            MenuItems =
            [
                new MenuItem { Dish = "Caprese Salad", Ingredient = tomatoes, QuantityRequired = 0.20m },
                new MenuItem { Dish = "Caprese Salad", Ingredient = lettuce, QuantityRequired = 0.08m },
                new MenuItem { Dish = "Caprese Salad", Ingredient = cheese, QuantityRequired = 0.05m },
            ],
        };
        db.Menus.AddRange(lunchMenu, saladMenu);

        var alice = new Employee { FirstName = "Alice", LastName = "Müller", Role = "Chef" };
        var bruno = new Employee { FirstName = "Bruno", LastName = "Rossi", Role = "Server" };
        var carla = new Employee { FirstName = "Carla", LastName = "Weber", Role = "Manager" };
        db.Employees.AddRange(alice, bruno, carla);

        // A week of shifts starting Monday of the current week.
        var monday = today.AddDays(-((int)today.DayOfWeek + 6) % 7);
        var morning = (Start: new TimeOnly(8, 0), End: new TimeOnly(16, 0));
        var evening = (Start: new TimeOnly(14, 0), End: new TimeOnly(22, 0));

        for (var offset = 0; offset < 5; offset++) // Mon–Fri
        {
            var date = monday.AddDays(offset);
            db.Shifts.Add(new Shift { Employee = alice, Date = date, StartTime = morning.Start, EndTime = morning.End });
            db.Shifts.Add(new Shift { Employee = bruno, Date = date, StartTime = evening.Start, EndTime = evening.End });
        }
        db.Shifts.Add(new Shift { Employee = carla, Date = monday, StartTime = morning.Start, EndTime = morning.End, Notes = "Weekly planning" });

        await db.SaveChangesAsync(ct);
    }
}
