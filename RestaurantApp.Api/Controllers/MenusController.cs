using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Api.DTOs;
using RestaurantApp.Api.Services;
using RestaurantApp.Core;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;

namespace RestaurantApp.Api.Controllers;

[ApiController]
[Route("api/menus")]
public class MenusController(
    IMenuRepository menus,
    IUnitOfWork unitOfWork,
    MenuPdfService pdf) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetAll(CancellationToken ct)
    {
        var all = await menus.GetAllAsync(ct);
        return Results.Ok(all.Select(m => m.ToSummaryDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetById(int id, CancellationToken ct)
    {
        var menu = await menus.GetWithRecipesAsync(id, ct);
        return menu is null ? Results.NotFound() : Results.Ok(menu.ToDto());
    }

    [HttpPost]
    public async Task<IResult> Create(CreateMenuRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.BadRequest(new { error = "Name is required." });

        var menu = new Menu
        {
            Name = request.Name,
            // Menus always run Monday–Sunday; snap whatever date was picked to its Monday.
            WeekStart = WeekRules.MondayOf(request.WeekStart),
        };

        menus.Add(menu);
        await unitOfWork.SaveChangesAsync(ct);

        var created = await menus.GetWithRecipesAsync(menu.Id, ct);
        return Results.Created($"/api/menus/{menu.Id}", created!.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IResult> Update(int id, UpdateMenuRequest request, CancellationToken ct)
    {
        var menu = await menus.GetWithRecipesAsync(id, ct);
        if (menu is null)
            return Results.NotFound();

        menu.Name = request.Name;
        menu.WeekStart = WeekRules.MondayOf(request.WeekStart);
        menu.Content = request.Content;
        menu.NutritionalInfo = request.NutritionalInfo;

        // Replace the recipe assignments wholesale.
        menu.MenuRecipes.Clear();
        foreach (var item in request.Recipes.Where(r => r.RecipeId > 0))
            menu.MenuRecipes.Add(new MenuRecipe { RecipeId = item.RecipeId, Day = Math.Clamp(item.Day, 0, 6) });

        await unitOfWork.SaveChangesAsync(ct);
        var updated = await menus.GetWithRecipesAsync(id, ct);
        return Results.Ok(updated!.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> Delete(int id, CancellationToken ct)
    {
        var menu = await menus.GetByIdAsync(id, ct);
        if (menu is null)
            return Results.NotFound();

        menus.Remove(menu);
        await unitOfWork.SaveChangesAsync(ct);
        return Results.NoContent();
    }

    /// <summary>Renders the menu as an A4-landscape PDF.</summary>
    [HttpGet("{id:int}/pdf")]
    public async Task<IResult> GetPdf(int id, CancellationToken ct)
    {
        var menu = await menus.GetWithRecipesAsync(id, ct);
        if (menu is null)
            return Results.NotFound();

        var bytes = pdf.Generate(menu);
        var fileName = $"menu-{menu.WeekStart:yyyy-MM-dd}.pdf";
        return Results.File(bytes, "application/pdf", fileName);
    }
}
