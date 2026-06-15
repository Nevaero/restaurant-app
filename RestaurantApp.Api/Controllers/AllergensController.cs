using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Core.Interfaces;

namespace RestaurantApp.Api.Controllers;

[ApiController]
[Route("api/allergens")]
public class AllergensController(IAllergenRepository allergens) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetAll(CancellationToken ct)
    {
        var all = await allergens.GetAllAsync(ct);
        return Results.Ok(all.Select(a => a.ToDto()));
    }
}
