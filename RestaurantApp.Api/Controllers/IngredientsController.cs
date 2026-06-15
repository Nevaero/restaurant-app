using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Api.DTOs;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;

namespace RestaurantApp.Api.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientsController(
    IIngredientRepository ingredients,
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
            Allergens = request.Allergens ?? string.Empty,
        };

        ingredients.Add(ingredient);
        await unitOfWork.SaveChangesAsync(ct);
        return Results.Created($"/api/ingredients/{ingredient.Id}", ingredient.ToDto());
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
        ingredient.Allergens = request.Allergens ?? string.Empty;

        await unitOfWork.SaveChangesAsync(ct);
        return Results.Ok(ingredient.ToDto());
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
