using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Api.DTOs;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;

namespace RestaurantApp.Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController(
    IEmployeeRepository employees,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetAll(CancellationToken ct)
    {
        var all = await employees.GetAllAsync(ct);
        return Results.Ok(all.Select(e => e.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetById(int id, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(id, ct);
        return employee is null ? Results.NotFound() : Results.Ok(employee.ToDto());
    }

    [HttpPost]
    public async Task<IResult> Create(CreateEmployeeRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return Results.BadRequest(new { error = "First and last name are required." });

        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            HourlyRate = request.HourlyRate,
        };

        employees.Add(employee);
        await unitOfWork.SaveChangesAsync(ct);
        return Results.Created($"/api/employees/{employee.Id}", employee.ToDto());
    }
}
