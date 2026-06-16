using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core;
using RestaurantApp.Core.Entities;

namespace RestaurantApp.Infrastructure.Data;

/// <summary>Seeds a realistic data set so every view (menus, recipes, inventory, planning) is populated.</summary>
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

        // ── Allergens (normalized reference data) ───────────────────────────────
        var a = EuAllergens.ToDictionary(name => name, name => new Allergen { Name = name });
        db.Allergens.AddRange(a.Values);

        // ── Inventory (a few items deliberately below threshold to show alerts) ──
        var tomatoes  = new Ingredient { Name = "Tomatoes",    Quantity = 25m,  Unit = "kg",     LowStockThreshold = 5m };
        var pasta     = new Ingredient { Name = "Pasta",       Quantity = 40m,  Unit = "kg",     LowStockThreshold = 10m, Allergens = [a["Gluten"]] };
        var beef      = new Ingredient { Name = "Ground Beef", Quantity = 12m,  Unit = "kg",     LowStockThreshold = 4m };
        var parmesan  = new Ingredient { Name = "Parmesan",    Quantity = 6m,   Unit = "kg",     LowStockThreshold = 2m,  Allergens = [a["Milk"]] };
        var pesto     = new Ingredient { Name = "Pesto",       Quantity = 3m,   Unit = "liters", LowStockThreshold = 2m,  Allergens = [a["Milk"], a["Nuts"]] };
        var eggs      = new Ingredient { Name = "Eggs",        Quantity = 200m, Unit = "units",  LowStockThreshold = 48m, Allergens = [a["Eggs"]] };
        var flour     = new Ingredient { Name = "Flour",       Quantity = 30m,  Unit = "kg",     LowStockThreshold = 8m,  Allergens = [a["Gluten"]] };
        var salmon    = new Ingredient { Name = "Salmon",      Quantity = 3m,   Unit = "kg",     LowStockThreshold = 5m,  Allergens = [a["Fish"]] };          // LOW
        var shrimp    = new Ingredient { Name = "Shrimp",      Quantity = 1.5m, Unit = "kg",     LowStockThreshold = 3m,  Allergens = [a["Crustaceans"]] };   // LOW
        var oliveOil  = new Ingredient { Name = "Olive Oil",   Quantity = 20m,  Unit = "liters", LowStockThreshold = 5m };
        var rice      = new Ingredient { Name = "Rice",        Quantity = 25m,  Unit = "kg",     LowStockThreshold = 8m };
        var butter    = new Ingredient { Name = "Butter",      Quantity = 4m,   Unit = "kg",     LowStockThreshold = 5m,  Allergens = [a["Milk"]] };          // LOW
        var lettuce   = new Ingredient { Name = "Lettuce",     Quantity = 8m,   Unit = "kg",     LowStockThreshold = 3m };
        var basil     = new Ingredient { Name = "Basil",       Quantity = 2m,   Unit = "kg",     LowStockThreshold = 1m };
        db.Ingredients.AddRange(tomatoes, pasta, beef, parmesan, pesto, eggs, flour,
            salmon, shrimp, oliveOil, rice, butter, lettuce, basil);

        // ── Recipes (built from inventory; allergens roll up automatically) ──────
        var bolognese = new Recipe
        {
            Name = "Pasta Bolognese", Servings = 4,
            Instructions = "Brown the beef, add tomatoes, simmer 30 min, serve over pasta with parmesan.",
            Ingredients =
            [
                new RecipeIngredient { Ingredient = pasta,    Quantity = 0.5m },
                new RecipeIngredient { Ingredient = beef,     Quantity = 0.6m },
                new RecipeIngredient { Ingredient = tomatoes, Quantity = 0.4m },
                new RecipeIngredient { Ingredient = parmesan, Quantity = 0.1m },
            ],
        };
        var pestoPasta = new Recipe
        {
            Name = "Pesto Pasta", Servings = 4,
            Instructions = "Cook pasta, toss with pesto, top with parmesan.",
            Ingredients =
            [
                new RecipeIngredient { Ingredient = pasta,    Quantity = 0.5m },
                new RecipeIngredient { Ingredient = pesto,    Quantity = 0.2m },
                new RecipeIngredient { Ingredient = parmesan, Quantity = 0.1m },
            ],
        };
        var grilledSalmon = new Recipe
        {
            Name = "Grilled Salmon", Servings = 2,
            Instructions = "Season salmon, grill 4 min per side, serve with dressed lettuce.",
            Ingredients =
            [
                new RecipeIngredient { Ingredient = salmon,   Quantity = 0.4m },
                new RecipeIngredient { Ingredient = oliveOil, Quantity = 0.05m },
                new RecipeIngredient { Ingredient = lettuce,  Quantity = 0.2m },
            ],
        };
        var shrimpRisotto = new Recipe
        {
            Name = "Shrimp Risotto", Servings = 4,
            Instructions = "Toast rice, add stock gradually, fold in shrimp, butter and parmesan.",
            Ingredients =
            [
                new RecipeIngredient { Ingredient = rice,     Quantity = 0.4m },
                new RecipeIngredient { Ingredient = shrimp,   Quantity = 0.3m },
                new RecipeIngredient { Ingredient = butter,   Quantity = 0.05m },
                new RecipeIngredient { Ingredient = parmesan, Quantity = 0.08m },
            ],
        };
        var caprese = new Recipe
        {
            Name = "Caprese Salad", Servings = 2,
            Instructions = "Slice tomatoes, layer with parmesan shavings and basil, drizzle with oil.",
            Ingredients =
            [
                new RecipeIngredient { Ingredient = tomatoes, Quantity = 0.3m },
                new RecipeIngredient { Ingredient = parmesan, Quantity = 0.05m },
                new RecipeIngredient { Ingredient = basil,    Quantity = 0.02m },
            ],
        };
        var omelette = new Recipe
        {
            Name = "Cheese Omelette", Servings = 1,
            Instructions = "Beat eggs, cook in butter, fold with parmesan.",
            Ingredients =
            [
                new RecipeIngredient { Ingredient = eggs,     Quantity = 3m },
                new RecipeIngredient { Ingredient = butter,   Quantity = 0.02m },
                new RecipeIngredient { Ingredient = parmesan, Quantity = 0.04m },
            ],
        };
        db.Recipes.AddRange(bolognese, pestoPasta, grilledSalmon, shrimpRisotto, caprese, omelette);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var thisMonday = WeekRules.MondayOf(today);

        // ── Menus (recipes placed across the week so the grid/PDF is full) ───────
        db.Menus.Add(new Menu
        {
            Name = "Spring lunch menu", WeekStart = thisMonday,
            Content = "House specials, served 11:30–14:00.",
            NutritionalInfo = "Avg. 750 kcal/serving · 35 g protein · 28 g fat · 80 g carbs",
            MenuRecipes =
            [
                new MenuRecipe { Recipe = bolognese,     Day = 0 },
                new MenuRecipe { Recipe = grilledSalmon, Day = 1 },
                new MenuRecipe { Recipe = pestoPasta,    Day = 2 },
                new MenuRecipe { Recipe = shrimpRisotto, Day = 3 },
                new MenuRecipe { Recipe = caprese,       Day = 4 },
            ],
        });
        db.Menus.Add(new Menu
        {
            Name = "Previous week", WeekStart = thisMonday.AddDays(-7),
            Content = "Rotating seasonal dishes.",
            NutritionalInfo = "Avg. 720 kcal/serving",
            MenuRecipes =
            [
                new MenuRecipe { Recipe = omelette,   Day = 0 },
                new MenuRecipe { Recipe = caprese,    Day = 2 },
                new MenuRecipe { Recipe = bolognese,  Day = 4 },
            ],
        });
        db.Menus.Add(new Menu
        {
            Name = "Next week (draft)", WeekStart = thisMonday.AddDays(7),
            Content = "Draft — to be confirmed.",
            NutritionalInfo = "",
            MenuRecipes = [new MenuRecipe { Recipe = pestoPasta, Day = 0 }],
        });

        // ── Staff ────────────────────────────────────────────────────────────────
        var alice = new Employee { FirstName = "Alice", LastName = "Müller", Role = "Cook",    HourlyRate = 34.50m };
        var bruno = new Employee { FirstName = "Bruno", LastName = "Rossi",  Role = "Clerk",   HourlyRate = 27.00m };
        var carla = new Employee { FirstName = "Carla", LastName = "Weber",  Role = "Manager", HourlyRate = 45.00m };
        var david = new Employee { FirstName = "David", LastName = "Schmid", Role = "Cook",    HourlyRate = 33.00m };
        var elena = new Employee { FirstName = "Elena", LastName = "Favre",  Role = "Server",  HourlyRate = 28.50m };
        db.Employees.AddRange(alice, bruno, carla, david, elena);

        // ── Shifts for the current week (Alice exceeds 42h → overtime flag) ──────
        void AddShifts(Employee e, int fromDay, int toDay, int startHour, int endHour, string? notes = null)
        {
            for (var d = fromDay; d <= toDay; d++)
                db.Shifts.Add(new Shift
                {
                    Employee = e,
                    Date = thisMonday.AddDays(d),
                    StartTime = new TimeOnly(startHour, 0),
                    EndTime = new TimeOnly(endHour, 0),
                    Notes = d == fromDay ? notes : null,
                });
        }

        AddShifts(alice, 0, 5, 8, 16, "Opening shift");  // Mon–Sat, 48h → overtime
        AddShifts(bruno, 0, 4, 14, 22);                   // Mon–Fri, 40h
        AddShifts(david, 1, 5, 10, 18);                   // Tue–Sat, 40h
        AddShifts(elena, 2, 6, 16, 22);                   // Wed–Sun, 30h
        db.Shifts.Add(new Shift { Employee = carla, Date = thisMonday,           StartTime = new(9, 0), EndTime = new(17, 0), Notes = "Weekly planning" });
        db.Shifts.Add(new Shift { Employee = carla, Date = thisMonday.AddDays(2), StartTime = new(9, 0), EndTime = new(17, 0) });
        db.Shifts.Add(new Shift { Employee = carla, Date = thisMonday.AddDays(4), StartTime = new(9, 0), EndTime = new(17, 0) });

        await db.SaveChangesAsync(ct);
    }
}
