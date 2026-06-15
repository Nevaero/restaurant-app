using System.Net;
using System.Net.Http.Json;
using RestaurantApp.Web.Models;

namespace RestaurantApp.Web.Services;

/// <summary>Typed wrapper over <see cref="HttpClient"/> for the RestaurantApp API.</summary>
public class ApiClient(HttpClient http)
{
    // Menus
    public async Task<List<MenuDto>> GetMenusAsync() =>
        await http.GetFromJsonAsync<List<MenuDto>>("api/menus") ?? [];

    public async Task<List<MenuDto>> GetTodaysMenusAsync() =>
        await http.GetFromJsonAsync<List<MenuDto>>("api/menus/today") ?? [];

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

    public async Task<ServeMenuResponse?> ServeMenuAsync(int id, int servings)
    {
        var response = await http.PostAsJsonAsync($"api/menus/{id}/serve", new ServeMenuRequest(servings));
        // 400 still carries a ServeMenuResponse body describing the shortfall.
        return await response.Content.ReadFromJsonAsync<ServeMenuResponse>();
    }

    // Ingredients
    public async Task<List<IngredientDto>> GetIngredientsAsync() =>
        await http.GetFromJsonAsync<List<IngredientDto>>("api/ingredients") ?? [];

    public async Task<List<IngredientDto>> GetLowStockAsync() =>
        await http.GetFromJsonAsync<List<IngredientDto>>("api/ingredients/low-stock") ?? [];

    public async Task<IngredientDto?> CreateIngredientAsync(CreateIngredientRequest request)
    {
        var response = await http.PostAsJsonAsync("api/ingredients", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IngredientDto>();
    }

    public async Task<IngredientDto?> UpdateStockAsync(int id, decimal stockQuantity)
    {
        var response = await http.PatchAsJsonAsync($"api/ingredients/{id}/stock", new UpdateStockRequest(stockQuantity));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IngredientDto>();
    }

    // Employees
    public async Task<List<EmployeeDto>> GetEmployeesAsync() =>
        await http.GetFromJsonAsync<List<EmployeeDto>>("api/employees") ?? [];

    // Shifts
    public async Task<List<ShiftDto>> GetWeekShiftsAsync(DateOnly weekStart) =>
        await http.GetFromJsonAsync<List<ShiftDto>>($"api/shifts/week?start={weekStart:yyyy-MM-dd}") ?? [];

    public async Task<(bool Success, string? Error)> CreateShiftAsync(CreateShiftRequest request)
    {
        var response = await http.PostAsJsonAsync("api/shifts", request);
        if (response.IsSuccessStatusCode)
            return (true, null);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var problem = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return (false, problem?.Error ?? "Could not create shift.");
        }
        return (false, $"Request failed: {(int)response.StatusCode}");
    }

    public async Task DeleteShiftAsync(int id) =>
        (await http.DeleteAsync($"api/shifts/{id}")).EnsureSuccessStatusCode();

    private record ErrorResponse(string Error);
}
