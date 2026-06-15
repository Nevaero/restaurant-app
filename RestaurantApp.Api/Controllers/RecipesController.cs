using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Api.DTOs;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;

namespace RestaurantApp.Api.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipesController(IRecipeRepository recipes, IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> Search([FromQuery] string? search, CancellationToken ct)
    {
        var found = await recipes.SearchAsync(search, ct);
        return Results.Ok(found.Select(r => r.ToSummaryDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetById(int id, CancellationToken ct)
    {
        var recipe = await recipes.GetByIdAsync(id, ct);
        return recipe is null ? Results.NotFound() : Results.Ok(recipe.ToDto());
    }

    [HttpPost]
    public async Task<IResult> Create(CreateRecipeRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.BadRequest(new { error = "Name is required." });

        var recipe = new Recipe
        {
            Name = request.Name,
            Instructions = request.Instructions,
            Servings = request.Servings < 1 ? 1 : request.Servings,
            Ingredients = BuildIngredients(request.Ingredients),
        };

        recipes.Add(recipe);
        await unitOfWork.SaveChangesAsync(ct);

        var created = await recipes.GetByIdAsync(recipe.Id, ct);
        return Results.Created($"/api/recipes/{recipe.Id}", created!.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IResult> Update(int id, UpdateRecipeRequest request, CancellationToken ct)
    {
        var recipe = await recipes.GetByIdAsync(id, ct);
        if (recipe is null)
            return Results.NotFound();

        recipe.Name = request.Name;
        recipe.Instructions = request.Instructions;
        recipe.Servings = request.Servings < 1 ? 1 : request.Servings;

        // Replace the ingredient set wholesale.
        recipe.Ingredients.Clear();
        foreach (var item in BuildIngredients(request.Ingredients))
            recipe.Ingredients.Add(item);

        await unitOfWork.SaveChangesAsync(ct);
        var updated = await recipes.GetByIdAsync(id, ct);
        return Results.Ok(updated!.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> Delete(int id, CancellationToken ct)
    {
        var recipe = await recipes.GetByIdAsync(id, ct);
        if (recipe is null)
            return Results.NotFound();

        recipes.Remove(recipe);
        await unitOfWork.SaveChangesAsync(ct);
        return Results.NoContent();
    }

    private static List<RecipeIngredient> BuildIngredients(IReadOnlyList<RecipeIngredientInput> inputs) =>
        inputs
            .Where(i => i.IngredientId > 0)
            .Select(i => new RecipeIngredient { IngredientId = i.IngredientId, Quantity = i.Quantity })
            .ToList();
}
