namespace RestaurantApp.Core.Entities;

/// <summary>A normalized allergen (e.g. one of the EU's 14 major allergens).</summary>
public class Allergen
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Ingredient> Ingredients { get; set; } = [];
}
