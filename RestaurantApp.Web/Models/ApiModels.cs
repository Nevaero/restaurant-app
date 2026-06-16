namespace RestaurantApp.Web.Models;

// Client-side mirrors of the API contract. A standalone WebAssembly client keeps its
// own DTOs rather than referencing the server project.

// Allergens (normalized)
public record AllergenDto(int Id, string Name);

// Per-serving nutritional values.
public record NutritionDto(int Calories, decimal Protein, decimal Carbohydrates, decimal Fat, decimal Sugars)
{
    public static NutritionDto Empty => new(0, 0, 0, 0, 0);
}

// Menus
public record MenuSummaryDto(int Id, string Name, DateOnly WeekStart);

public record MenuDayDto(int Day, string Dish, int? RecipeId, NutritionDto Nutrition, IReadOnlyList<string> Allergens);

public record MenuDto(
    int Id,
    string Name,
    DateOnly WeekStart,
    string Content,
    IReadOnlyList<MenuDayDto> Days,
    IReadOnlyList<string> Allergens);

public record CreateMenuRequest(string Name, DateOnly WeekStart);

public record MenuDayInput(int Day, string Dish, int? RecipeId, NutritionDto Nutrition);

public record UpdateMenuRequest(
    string Name,
    DateOnly WeekStart,
    string Content,
    IReadOnlyList<MenuDayInput> Days);

// Recipes
public record RecipeIngredientDto(int IngredientId, string IngredientName, string Unit, decimal Quantity);

public record RecipeDto(int Id, string Name, string Instructions, int Servings, NutritionDto Nutrition, IReadOnlyList<RecipeIngredientDto> Ingredients, IReadOnlyList<string> Allergens);

public record RecipeSummaryDto(int Id, string Name, int Servings, int IngredientCount, NutritionDto Nutrition, IReadOnlyList<string> Allergens);

public record RecipeIngredientInput(int IngredientId, decimal Quantity);

public record CreateRecipeRequest(string Name, string Instructions, int Servings, NutritionDto Nutrition, IReadOnlyList<RecipeIngredientInput> Ingredients);

public record UpdateRecipeRequest(string Name, string Instructions, int Servings, NutritionDto Nutrition, IReadOnlyList<RecipeIngredientInput> Ingredients);

// Inventory
public record IngredientDto(int Id, string Name, decimal Quantity, string Unit, decimal LowStockThreshold, IReadOnlyList<AllergenDto> Allergens, bool IsLow);

public record CreateIngredientRequest(string Name, decimal Quantity, string Unit, decimal LowStockThreshold, IReadOnlyList<int> AllergenIds);

public record UpdateIngredientRequest(string Name, decimal Quantity, string Unit, decimal LowStockThreshold, IReadOnlyList<int> AllergenIds);

// Staff & planning
public record EmployeeDto(int Id, string FirstName, string LastName, string Role, decimal HourlyRate)
{
    public string FullName => $"{FirstName} {LastName}";
}

public record CreateEmployeeRequest(string FirstName, string LastName, string Role, decimal HourlyRate);

public record ShiftDto(int Id, int EmployeeId, string EmployeeName, string Role, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string? Notes);

public record CreateShiftRequest(int EmployeeId, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string? Notes);

public record EmployeeWeekSummaryDto(int EmployeeId, string Name, string Role, decimal TotalHours, decimal LaborCost, bool IsOvertime);

public record WeekScheduleDto(DateOnly WeekStart, IReadOnlyList<ShiftDto> Shifts, IReadOnlyList<EmployeeWeekSummaryDto> Summaries);
