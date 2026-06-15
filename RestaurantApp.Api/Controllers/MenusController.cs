using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Api.DTOs;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Core.Services;

namespace RestaurantApp.Api.Controllers;

[ApiController]
[Route("api/menus")]
public class MenusController(
    IMenuRepository menus,
    IUnitOfWork unitOfWork,
    StockDeductionService stockDeduction) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetAll(CancellationToken ct)
    {
        var all = await menus.GetAllAsync(ct);
        return Results.Ok(all.Select(m => m.ToDto()));
    }

    [HttpGet("today")]
    public async Task<IResult> GetToday(CancellationToken ct)
    {
        var today = await menus.GetActiveByDateAsync(DateOnly.FromDateTime(DateTime.Today), ct);
        return Results.Ok(today.Select(m => m.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetById(int id, CancellationToken ct)
    {
        var menu = await menus.GetWithItemsAsync(id, ct);
        return menu is null ? Results.NotFound() : Results.Ok(menu.ToDto());
    }

    [HttpPost]
    public async Task<IResult> Create(CreateMenuRequest request, CancellationToken ct)
    {
        var menu = new Menu
        {
            Name = request.Name,
            Date = request.Date,
            IsActive = request.IsActive,
            MenuItems = request.Items
                .Select(i => new MenuItem
                {
                    Dish = i.Dish,
                    IngredientId = i.IngredientId,
                    QuantityRequired = i.QuantityRequired,
                })
                .ToList(),
        };

        menus.Add(menu);
        await unitOfWork.SaveChangesAsync(ct);

        var created = await menus.GetWithItemsAsync(menu.Id, ct);
        return Results.Created($"/api/menus/{menu.Id}", created!.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IResult> Update(int id, UpdateMenuRequest request, CancellationToken ct)
    {
        var menu = await menus.GetWithItemsAsync(id, ct);
        if (menu is null)
            return Results.NotFound();

        menu.Name = request.Name;
        menu.Date = request.Date;
        menu.IsActive = request.IsActive;

        // Replace the item set wholesale — simplest correct behaviour for the demo.
        menu.MenuItems.Clear();
        foreach (var item in request.Items)
        {
            menu.MenuItems.Add(new MenuItem
            {
                Dish = item.Dish,
                IngredientId = item.IngredientId,
                QuantityRequired = item.QuantityRequired,
            });
        }

        await unitOfWork.SaveChangesAsync(ct);
        var updated = await menus.GetWithItemsAsync(id, ct);
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

    /// <summary>Serves a menu, deducting ingredient stock inside a single transaction.</summary>
    [HttpPost("{id:int}/serve")]
    public async Task<IResult> Serve(int id, ServeMenuRequest request, CancellationToken ct)
    {
        var result = await stockDeduction.ServeMenuAsync(id, request.Servings, ct);

        var warnings = result.Warnings
            .Select(w => new LowStockWarningDto(w.IngredientId, w.Name, w.RemainingStock, w.Threshold, w.Unit))
            .ToList();
        var response = new ServeMenuResponse(result.Success, result.Error, warnings);

        return result.Success ? Results.Ok(response) : Results.BadRequest(response);
    }
}
