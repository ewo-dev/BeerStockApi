using BeerStockApi.Domain;

namespace BeerStockApi.Repositories;

public interface IBeerRepository
{
    Task<List<Beer>> GetAllAsync();
    Task<Beer?> GetByIdAsync(int id);
    Task<List<Beer>> GetByBrewerIdAsync(int brewerId);
    Task<Beer> CreateAsync(Beer beer);
    Task<Beer> UpdateAsync(Beer beer);
    Task DeleteAsync(int id);
    Task SaveAsync();
}
