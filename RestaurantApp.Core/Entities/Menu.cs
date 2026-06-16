namespace RestaurantApp.Core.Entities;

/// <summary>A weekly menu. Menus always run Monday–Sunday; <see cref="WeekStart"/> is the Monday.</summary>
public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;     // e.g. "Menu de printemps"
    public DateOnly WeekStart { get; set; }              // always a Monday
    public string Content { get; set; } = string.Empty;  // free-text notes

    public ICollection<MenuDay> Days { get; set; } = [];
}

/// <summary>One day of a menu: the dish and its nutritional values, optionally imported from a recipe.</summary>
public class MenuDay
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public Menu Menu { get; set; } = null!;

    /// <summary>Day offset from the menu's Monday: 0 = Monday … 6 = Sunday.</summary>
    public int Day { get; set; }

    public string Dish { get; set; } = string.Empty;

    /// <summary>Recipe the values were imported from (kept so allergens can roll up). Null if hand-entered.</summary>
    public int? RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    // Nutritional values for the day's serving.
    public int Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbohydrates { get; set; }
    public decimal Fat { get; set; }
    public decimal Sugars { get; set; }
}
