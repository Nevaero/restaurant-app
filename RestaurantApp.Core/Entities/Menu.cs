namespace RestaurantApp.Core.Entities;

/// <summary>A weekly menu. Menus always run Monday–Sunday; <see cref="WeekStart"/> is the Monday.</summary>
public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;          // e.g. "Week 24 – Spring menu"
    public DateOnly WeekStart { get; set; }                    // always a Monday
    public string Content { get; set; } = string.Empty;        // free-text notes
    public string NutritionalInfo { get; set; } = string.Empty; // calories, macros, notes

    public ICollection<MenuRecipe> MenuRecipes { get; set; } = [];
}

/// <summary>Join entity: a recipe placed on a given day of a menu's week.</summary>
public class MenuRecipe
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public Menu Menu { get; set; } = null!;
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    /// <summary>Day offset from the menu's Monday: 0 = Monday … 6 = Sunday.</summary>
    public int Day { get; set; }
}
