using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext db) : IEmployeeRepository
{
    public Task<List<Employee>> GetAllAsync(CancellationToken ct = default) =>
        db.Employees
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .ToListAsync(ct);

    public Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);

    public void Add(Employee employee) => db.Employees.Add(employee);
}
