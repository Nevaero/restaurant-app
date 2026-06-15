using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core;
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

        // Inventory
        db.Ingredients.AddRange(
            new Ingredient { Name = "Tomatoes", Quantity = 25m, Unit = "kg", LowStockThreshold = 5m, Allergens = "" },
            new Ingredient { Name = "Pasta", Quantity = 40m, Unit = "kg", LowStockThreshold = 10m, Allergens = "Gluten" },
            new Ingredient { Name = "Ground Beef", Quantity = 12m, Unit = "kg", LowStockThreshold = 4m, Allergens = "" },
            new Ingredient { Name = "Parmesan", Quantity = 6m, Unit = "kg", LowStockThreshold = 2m, Allergens = "Milk" },
            new Ingredient { Name = "Pesto", Quantity = 3m, Unit = "liters", LowStockThreshold = 2m, Allergens = "Milk,Nuts" });

        var today = DateOnly.FromDateTime(DateTime.Today);
        var thisMonday = WeekRules.MondayOf(today);

        // Menus (Monday-anchored, free-text)
        db.Menus.AddRange(
            new Menu
            {
                Name = "Spring lunch menu",
                WeekStart = thisMonday,
                Content =
                    "Monday: Pasta Bolognese · Green salad\n" +
                    "Tuesday: Chicken curry · Basmati rice\n" +
                    "Wednesday: Vegetarian lasagne\n" +
                    "Thursday: Grilled salmon · Seasonal vegetables\n" +
                    "Friday: Beef burger · Fries",
                NutritionalInfo = "Avg. 750 kcal/serving · 35 g protein · 28 g fat · 80 g carbs",
            },
            new Menu
            {
                Name = "Previous week",
                WeekStart = thisMonday.AddDays(-7),
                Content = "Monday–Friday: rotating seasonal dishes.",
                NutritionalInfo = "Avg. 720 kcal/serving",
            });

        // Staff
        var alice = new Employee { FirstName = "Alice", LastName = "Müller", Role = "Cook", HourlyRate = 32.00m };
        var bruno = new Employee { FirstName = "Bruno", LastName = "Rossi", Role = "Clerk", HourlyRate = 26.50m };
        var carla = new Employee { FirstName = "Carla", LastName = "Weber", Role = "Manager", HourlyRate = 41.00m };
        db.Employees.AddRange(alice, bruno, carla);

        var morning = (Start: new TimeOnly(8, 0), End: new TimeOnly(16, 0));
        var evening = (Start: new TimeOnly(14, 0), End: new TimeOnly(22, 0));

        for (var offset = 0; offset < 5; offset++) // Mon–Fri
        {
            var date = thisMonday.AddDays(offset);
            db.Shifts.Add(new Shift { Employee = alice, Date = date, StartTime = morning.Start, EndTime = morning.End });
            db.Shifts.Add(new Shift { Employee = bruno, Date = date, StartTime = evening.Start, EndTime = evening.End });
        }
        db.Shifts.Add(new Shift { Employee = carla, Date = thisMonday, StartTime = morning.Start, EndTime = morning.End, Notes = "Weekly planning" });

        await db.SaveChangesAsync(ct);
    }
}
