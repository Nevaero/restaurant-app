using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Repositories;

public class MenuRepository(AppDbContext db) : IMenuRepository
{
    public Task<List<Menu>> GetAllAsync(CancellationToken ct = default) =>
        db.Menus.OrderByDescending(m => m.WeekStart).ToListAsync(ct);

    public Task<Menu?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Menus.FirstOrDefaultAsync(m => m.Id == id, ct);

    public void Add(Menu menu) => db.Menus.Add(menu);

    public void Remove(Menu menu) => db.Menus.Remove(menu);
}
