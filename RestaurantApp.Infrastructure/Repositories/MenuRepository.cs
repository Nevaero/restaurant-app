using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Repositories;

public class MenuRepository(AppDbContext db) : IMenuRepository
{
    public Task<List<Menu>> GetAllAsync(CancellationToken ct = default) =>
        db.Menus
            .Include(m => m.MenuItems)
            .ThenInclude(i => i.Ingredient)
            .OrderByDescending(m => m.Date)
            .ToListAsync(ct);

    public Task<List<Menu>> GetActiveByDateAsync(DateOnly date, CancellationToken ct = default) =>
        db.Menus
            .Include(m => m.MenuItems)
            .ThenInclude(i => i.Ingredient)
            .Where(m => m.IsActive && m.Date == date)
            .ToListAsync(ct);

    public Task<Menu?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Menus
            .Include(m => m.MenuItems)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public Task<Menu?> GetWithItemsAsync(int id, CancellationToken ct = default) =>
        db.Menus
            .Include(m => m.MenuItems)
            .ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public void Add(Menu menu) => db.Menus.Add(menu);

    public void Remove(Menu menu) => db.Menus.Remove(menu);
}
