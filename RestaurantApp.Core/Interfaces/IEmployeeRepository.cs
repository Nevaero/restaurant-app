using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync(CancellationToken ct = default);
    Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default);
    void Add(Employee employee);
}
