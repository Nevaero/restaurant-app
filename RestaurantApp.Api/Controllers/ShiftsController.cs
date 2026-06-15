using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Api.DTOs;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;

namespace RestaurantApp.Api.Controllers;

[ApiController]
[Route("api/shifts")]
public class ShiftsController(
    IShiftRepository shifts,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetAll(CancellationToken ct)
    {
        var all = await shifts.GetAllAsync(ct);
        return Results.Ok(all.Select(s => s.ToDto()));
    }

    [HttpGet("week")]
    public async Task<IResult> GetWeek([FromQuery] DateOnly start, CancellationToken ct)
    {
        var week = await shifts.GetWeekAsync(start, ct);
        return Results.Ok(week.Select(s => s.ToDto()));
    }

    [HttpGet("employee/{employeeId:int}")]
    public async Task<IResult> GetByEmployee(int employeeId, CancellationToken ct)
    {
        var result = await shifts.GetByEmployeeAsync(employeeId, ct);
        return Results.Ok(result.Select(s => s.ToDto()));
    }

    [HttpPost]
    public async Task<IResult> Create(CreateShiftRequest request, CancellationToken ct)
    {
        if (request.EndTime <= request.StartTime)
            return Results.BadRequest(new { error = "End time must be after start time." });

        var shift = new Shift
        {
            EmployeeId = request.EmployeeId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Notes = request.Notes,
        };

        shifts.Add(shift);
        await unitOfWork.SaveChangesAsync(ct);

        var created = await shifts.GetByIdAsync(shift.Id, ct);
        return Results.Created($"/api/shifts/{shift.Id}", created!.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IResult> Update(int id, UpdateShiftRequest request, CancellationToken ct)
    {
        if (request.EndTime <= request.StartTime)
            return Results.BadRequest(new { error = "End time must be after start time." });

        var shift = await shifts.GetByIdAsync(id, ct);
        if (shift is null)
            return Results.NotFound();

        shift.EmployeeId = request.EmployeeId;
        shift.Date = request.Date;
        shift.StartTime = request.StartTime;
        shift.EndTime = request.EndTime;
        shift.Notes = request.Notes;

        await unitOfWork.SaveChangesAsync(ct);
        var updated = await shifts.GetByIdAsync(id, ct);
        return Results.Ok(updated!.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> Delete(int id, CancellationToken ct)
    {
        var shift = await shifts.GetByIdAsync(id, ct);
        if (shift is null)
            return Results.NotFound();

        shifts.Remove(shift);
        await unitOfWork.SaveChangesAsync(ct);
        return Results.NoContent();
    }
}
