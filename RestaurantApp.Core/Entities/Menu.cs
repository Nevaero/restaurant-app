namespace RestaurantApp.Core.Entities;

/// <summary>A daily menu offered at the restaurant.</summary>
public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;      // e.g. "Lunch Menu #3"
    public DateOnly Date { get; set; }
    public bool IsActive { get; set; }
    public ICollection<MenuItem> MenuItems { get; set; } = [];
}
