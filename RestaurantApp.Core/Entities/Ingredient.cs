namespace RestaurantApp.Core.Entities;

/// <summary>An inventory item held in the kitchen.</summary>
public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;     // e.g. "Tomatoes"
    public decimal Quantity { get; set; }                // quantity in stock
    public string Unit { get; set; } = string.Empty;     // "kg", "units", "liters"
    public decimal LowStockThreshold { get; set; }       // alert below this

    public ICollection<Allergen> Allergens { get; set; } = [];
}
