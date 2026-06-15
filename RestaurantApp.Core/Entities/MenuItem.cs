namespace RestaurantApp.Core.Entities;

/// <summary>Join entity: which ingredient a dish uses, and how much per serving.</summary>
public class MenuItem
{
    public int Id { get; set; }
    public string Dish { get; set; } = string.Empty;      // e.g. "Pasta Bolognese"
    public int MenuId { get; set; }
    public Menu Menu { get; set; } = null!;
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public decimal QuantityRequired { get; set; }         // per serving
}
