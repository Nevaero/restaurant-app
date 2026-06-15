using System.Net;
using System.Net.Http.Json;
using RestaurantApp.Web.Models;

namespace RestaurantApp.Web.Services;

/// <summary>Typed wrapper over <see cref="HttpClient"/> for the RestaurantApp API.</summary>
public class ApiClient(HttpClient http)
{
    /// <summary>Absolute URL for a menu's PDF (used as a download link target).</summary>
    public string MenuPdfUrl(int menuId) => new Uri(http.BaseAddress!, $"api/menus/{menuId}/pdf").ToString();

    // Menus
    public async Task<List<MenuSummaryDto>> GetMenusAsync() =>
        await http.GetFromJsonAsync<List<MenuSummaryDto>>("api/menus") ?? [];

    public async Task<MenuDto?> GetMenuAsync(int id) =>
        await http.GetFromJsonAsync<MenuDto>($"api/menus/{id}");

    public async Task<MenuDto?> CreateMenuAsync(CreateMenuRequest request)
    {
        var response = await http.PostAsJsonAsync("api/menus", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MenuDto>();
    }

    public async Task<MenuDto?> UpdateMenuAsync(int id, UpdateMenuRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/menus/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MenuDto>();
    }

    public async Task DeleteMenuAsync(int id) =>
        (await http.DeleteAsync($"api/menus/{id}")).EnsureSuccessStatusCode();

    // Recipes
    public async Task<List<RecipeSummaryDto>> SearchRecipesAsync(string? search)
    {
        var url = string.IsNullOrWhiteSpace(search) ? "api/recipes" : $"api/recipes?search={Uri.EscapeDataString(search)}";
        return await http.GetFromJsonAsync<List<RecipeSummaryDto>>(url) ?? [];
    }

    public async Task<RecipeDto?> GetRecipeAsync(int id) =>
        await http.GetFromJsonAsync<RecipeDto>($"api/recipes/{id}");

    public async Task<RecipeDto?> CreateRecipeAsync(CreateRecipeRequest request)
    {
        var response = await http.PostAsJsonAsync("api/recipes", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RecipeDto>();
    }

    public async Task<RecipeDto?> UpdateRecipeAsync(int id, UpdateRecipeRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/recipes/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RecipeDto>();
    }

    public async Task DeleteRecipeAsync(int id) =>
        (await http.DeleteAsync($"api/recipes/{id}")).EnsureSuccessStatusCode();

    // Allergens
    public async Task<List<AllergenDto>> GetAllergensAsync() =>
        await http.GetFromJsonAsync<List<AllergenDto>>("api/allergens") ?? [];

    // Inventory
    public async Task<List<IngredientDto>> GetIngredientsAsync() =>
        await http.GetFromJsonAsync<List<IngredientDto>>("api/ingredients") ?? [];

    public async Task<IngredientDto?> CreateIngredientAsync(CreateIngredientRequest request)
    {
        var response = await http.PostAsJsonAsync("api/ingredients", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IngredientDto>();
    }

    public async Task<IngredientDto?> UpdateIngredientAsync(int id, UpdateIngredientRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/ingredients/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IngredientDto>();
    }

    public async Task DeleteIngredientAsync(int id) =>
        (await http.DeleteAsync($"api/ingredients/{id}")).EnsureSuccessStatusCode();

    // Employees
    public async Task<List<EmployeeDto>> GetEmployeesAsync() =>
        await http.GetFromJsonAsync<List<EmployeeDto>>("api/employees") ?? [];

    public async Task<(bool Success, string? Error)> CreateEmployeeAsync(CreateEmployeeRequest request) =>
        await PostExpectingValidation("api/employees", request);

    // Shifts / planning
    public async Task<WeekScheduleDto?> GetWeekScheduleAsync(DateOnly weekStart) =>
        await http.GetFromJsonAsync<WeekScheduleDto>($"api/shifts/week/schedule?start={weekStart:yyyy-MM-dd}");

    public async Task<(bool Success, string? Error)> CreateShiftAsync(CreateShiftRequest request) =>
        await PostExpectingValidation("api/shifts", request);

    public async Task DeleteShiftAsync(int id) =>
        (await http.DeleteAsync($"api/shifts/{id}")).EnsureSuccessStatusCode();

    /// <summary>POSTs a request that may return a 400 with an { error } body.</summary>
    private async Task<(bool Success, string? Error)> PostExpectingValidation<T>(string url, T request)
    {
        var response = await http.PostAsJsonAsync(url, request);
        if (response.IsSuccessStatusCode)
            return (true, null);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var problem = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return (false, problem?.Error ?? "Request was rejected.");
        }
        return (false, $"Request failed: {(int)response.StatusCode}");
    }

    private record ErrorResponse(string Error);
}
