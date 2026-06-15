using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IShiftRepository
{
    Task<List<Shift>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Shifts in the 7-day window starting on <paramref name="weekStart"/> (inclusive).</summary>
    Task<List<Shift>> GetWeekAsync(DateOnly weekStart, CancellationToken ct = default);

    Task<List<Shift>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default);
    Task<Shift?> GetByIdAsync(int id, CancellationToken ct = default);
    void Add(Shift shift);
    void Remove(Shift shift);
}
