using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Api.DTOs;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;

namespace RestaurantApp.Api.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientsController(
    IIngredientRepository ingredients,
    IAllergenRepository allergens,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetAll(CancellationToken ct)
    {
        var all = await ingredients.GetAllAsync(ct);
        return Results.Ok(all.Select(i => i.ToDto()));
    }

    [HttpGet("low-stock")]
    public async Task<IResult> GetLowStock(CancellationToken ct)
    {
        var low = await ingredients.GetLowStockAsync(ct);
        return Results.Ok(low.Select(i => i.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetById(int id, CancellationToken ct)
    {
        var ingredient = await ingredients.GetByIdAsync(id, ct);
        return ingredient is null ? Results.NotFound() : Results.Ok(ingredient.ToDto());
    }

    [HttpPost]
    public async Task<IResult> Create(CreateIngredientRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.BadRequest(new { error = "Name is required." });

        var ingredient = new Ingredient
        {
            Name = request.Name,
            Quantity = request.Quantity,
            Unit = request.Unit,
            LowStockThreshold = request.LowStockThreshold,
            Allergens = await allergens.GetByIdsAsync(request.AllergenIds, ct),
        };

        ingredients.Add(ingredient);
        await unitOfWork.SaveChangesAsync(ct);

        var created = await ingredients.GetByIdAsync(ingredient.Id, ct);
        return Results.Created($"/api/ingredients/{ingredient.Id}", created!.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IResult> Update(int id, UpdateIngredientRequest request, CancellationToken ct)
    {
        var ingredient = await ingredients.GetByIdAsync(id, ct);
        if (ingredient is null)
            return Results.NotFound();

        ingredient.Name = request.Name;
        ingredient.Quantity = request.Quantity;
        ingredient.Unit = request.Unit;
        ingredient.LowStockThreshold = request.LowStockThreshold;

        var resolved = await allergens.GetByIdsAsync(request.AllergenIds, ct);
        ingredient.Allergens.Clear();
        foreach (var allergen in resolved)
            ingredient.Allergens.Add(allergen);

        await unitOfWork.SaveChangesAsync(ct);
        var updated = await ingredients.GetByIdAsync(id, ct);
        return Results.Ok(updated!.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> Delete(int id, CancellationToken ct)
    {
        var ingredient = await ingredients.GetByIdAsync(id, ct);
        if (ingredient is null)
            return Results.NotFound();

        ingredients.Remove(ingredient);
        await unitOfWork.SaveChangesAsync(ct);
        return Results.NoContent();
    }
}
