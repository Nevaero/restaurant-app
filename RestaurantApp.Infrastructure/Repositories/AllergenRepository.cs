using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Repositories;

public class AllergenRepository(AppDbContext db) : IAllergenRepository
{
    public Task<List<Allergen>> GetAllAsync(CancellationToken ct = default) =>
        db.Allergens.OrderBy(a => a.Name).ToListAsync(ct);

    public Task<List<Allergen>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var idSet = ids.Distinct().ToList();
        return db.Allergens.Where(a => idSet.Contains(a.Id)).ToListAsync(ct);
    }
}
