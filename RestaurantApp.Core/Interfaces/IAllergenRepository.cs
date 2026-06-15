using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Interfaces;

public interface IAllergenRepository
{
    Task<List<Allergen>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Loads the allergen entities for the given ids (for assigning to ingredients).</summary>
    Task<List<Allergen>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
}
