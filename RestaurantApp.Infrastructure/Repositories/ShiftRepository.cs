using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Repositories;

public class ShiftRepository(AppDbContext db) : IShiftRepository
{
    public Task<List<Shift>> GetAllAsync(CancellationToken ct = default) =>
        db.Shifts
            .Include(s => s.Employee)
            .OrderBy(s => s.Date).ThenBy(s => s.StartTime)
            .ToListAsync(ct);

    public Task<List<Shift>> GetWeekAsync(DateOnly weekStart, CancellationToken ct = default)
    {
        var weekEnd = weekStart.AddDays(6);
        return db.Shifts
            .Include(s => s.Employee)
            .Where(s => s.Date >= weekStart && s.Date <= weekEnd)
            .OrderBy(s => s.Date).ThenBy(s => s.StartTime)
            .ToListAsync(ct);
    }

    public Task<List<Shift>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default) =>
        db.Shifts
            .Include(s => s.Employee)
            .Where(s => s.EmployeeId == employeeId)
            .OrderBy(s => s.Date).ThenBy(s => s.StartTime)
            .ToListAsync(ct);

    public Task<Shift?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Shifts
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public void Add(Shift shift) => db.Shifts.Add(shift);

    public void Remove(Shift shift) => db.Shifts.Remove(shift);
}
