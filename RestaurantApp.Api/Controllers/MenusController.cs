using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Api.DTOs;
using RestaurantApp.Core;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;

namespace RestaurantApp.Api.Controllers;

[ApiController]
[Route("api/menus")]
public class MenusController(IMenuRepository menus, IUnitOfWork unitOfWork) : ControllerBase
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
        var menu = await menus.GetByIdAsync(id, ct);
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
            Content = string.Empty,
            NutritionalInfo = string.Empty,
        };

        menus.Add(menu);
        await unitOfWork.SaveChangesAsync(ct);
        return Results.Created($"/api/menus/{menu.Id}", menu.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IResult> Update(int id, UpdateMenuRequest request, CancellationToken ct)
    {
        var menu = await menus.GetByIdAsync(id, ct);
        if (menu is null)
            return Results.NotFound();

        menu.Name = request.Name;
        menu.WeekStart = WeekRules.MondayOf(request.WeekStart);
        menu.Content = request.Content;
        menu.NutritionalInfo = request.NutritionalInfo;

        await unitOfWork.SaveChangesAsync(ct);
        return Results.Ok(menu.ToDto());
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
}
