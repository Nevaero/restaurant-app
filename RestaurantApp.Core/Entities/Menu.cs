namespace RestaurantApp.Core.Entities;

/// <summary>A weekly menu. Menus always run Monday–Sunday; <see cref="WeekStart"/> is the Monday.</summary>
public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;          // e.g. "Week 24 – Spring menu"
    public DateOnly WeekStart { get; set; }                    // always a Monday
    public string Content { get; set; } = string.Empty;        // free-text menu (dishes per day, etc.)
    public string NutritionalInfo { get; set; } = string.Empty; // calories, macros, notes
}
