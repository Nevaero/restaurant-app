namespace RestaurantApp.Core.Entities;

/// <summary>A dish recipe built from inventory ingredients, with per-serving nutrition.</summary>
public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;        // e.g. "Pâtes à la bolognaise"
    public string Instructions { get; set; } = string.Empty; // preparation steps
    public int Servings { get; set; } = 1;                   // yields this many servings

    // Nutrition per serving.
    public int Calories { get; set; }                        // kcal
    public decimal Protein { get; set; }                     // g
    public decimal Carbohydrates { get; set; }               // g
    public decimal Fat { get; set; }                         // g
    public decimal Sugars { get; set; }                      // g

    public ICollection<RecipeIngredient> Ingredients { get; set; } = [];
}

/// <summary>Join entity: how much of an ingredient a recipe uses.</summary>
public class RecipeIngredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public decimal Quantity { get; set; }                    // in the ingredient's own unit
}
