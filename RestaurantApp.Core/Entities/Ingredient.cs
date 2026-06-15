namespace RestaurantApp.Core.Entities;

/// <summary>A stock item in the kitchen.</summary>
public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;      // e.g. "Tomatoes"
    public decimal StockQuantity { get; set; }            // current stock in kg/units
    public decimal LowStockThreshold { get; set; }        // alert below this
    public string Unit { get; set; } = string.Empty;      // "kg", "units", "liters"
    public ICollection<MenuItem> MenuItems { get; set; } = [];
}
