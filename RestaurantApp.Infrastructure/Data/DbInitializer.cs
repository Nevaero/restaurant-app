using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core;
using RestaurantApp.Core.Entities;

namespace RestaurantApp.Infrastructure.Data;

/// <summary>Seeds a small, realistic data set so the app is usable on first run.</summary>
public static class DbInitializer
{
    // The EU's 14 major food allergens.
    private static readonly string[] EuAllergens =
    [
        "Gluten", "Crustaceans", "Eggs", "Fish", "Peanuts", "Soybeans", "Milk",
        "Nuts", "Celery", "Mustard", "Sesame", "Sulphites", "Lupin", "Molluscs"
    ];

    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (await db.Allergens.AnyAsync(ct))
            return; // already seeded

        // Allergens (normalized reference data)
        var allergens = EuAllergens.ToDictionary(name => name, name => new Allergen { Name = name });
        db.Allergens.AddRange(allergens.Values);

        // Inventory, with allergen associations
        var tomatoes = new Ingredient { Name = "Tomatoes", Quantity = 25m, Unit = "kg", LowStockThreshold = 5m };
        var pasta = new Ingredient { Name = "Pasta", Quantity = 40m, Unit = "kg", LowStockThreshold = 10m, Allergens = [allergens["Gluten"]] };
        var beef = new Ingredient { Name = "Ground Beef", Quantity = 12m, Unit = "kg", LowStockThreshold = 4m };
        var parmesan = new Ingredient { Name = "Parmesan", Quantity = 6m, Unit = "kg", LowStockThreshold = 2m, Allergens = [allergens["Milk"]] };
        var pesto = new Ingredient { Name = "Pesto", Quantity = 3m, Unit = "liters", LowStockThreshold = 2m, Allergens = [allergens["Milk"], allergens["Nuts"]] };
        db.Ingredients.AddRange(tomatoes, pasta, beef, parmesan, pesto);

        // Recipes built from inventory
        var bolognese = new Recipe
        {
            Name = "Pasta Bolognese",
            Instructions = "Brown the beef, add tomatoes, simmer 30 min, serve over pasta with parmesan.",
            Servings = 4,
            Ingredients =
            [
                new RecipeIngredient { Ingredient = pasta, Quantity = 0.5m },
                new RecipeIngredient { Ingredient = beef, Quantity = 0.6m },
                new RecipeIngredient { Ingredient = tomatoes, Quantity = 0.4m },
                new RecipeIngredient { Ingredient = parmesan, Quantity = 0.1m },
            ],
        };
        var pestoPasta = new Recipe
        {
            Name = "Pesto Pasta",
            Instructions = "Cook pasta, toss with pesto, top with parmesan.",
            Servings = 4,
            Ingredients =
            [
                new RecipeIngredient { Ingredient = pasta, Quantity = 0.5m },
                new RecipeIngredient { Ingredient = pesto, Quantity = 0.2m },
                new RecipeIngredient { Ingredient = parmesan, Quantity = 0.1m },
            ],
        };
        db.Recipes.AddRange(bolognese, pestoPasta);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var thisMonday = WeekRules.MondayOf(today);

        db.Menus.Add(new Menu
        {
            Name = "Spring lunch menu",
            WeekStart = thisMonday,
            Content = "House specials this week.",
            NutritionalInfo = "Avg. 750 kcal/serving",
            MenuRecipes =
            [
                new MenuRecipe { Recipe = bolognese, Day = 0 }, // Monday
                new MenuRecipe { Recipe = pestoPasta, Day = 2 }, // Wednesday
            ],
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
