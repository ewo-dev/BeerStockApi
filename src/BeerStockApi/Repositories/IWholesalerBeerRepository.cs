using BeerStockApi.Domain;

namespace BeerStockApi.Repositories;

public interface IWholesalerBeerRepository
{
    Task<WholesalerBeer?> GetStockAsync(int wholesalerId, int beerId);
    Task<List<WholesalerBeer>> GetByWholesalerIdAsync(int wholesalerId);
    Task<List<WholesalerBeer>> GetByBeerIdAsync(int beerId);
    Task<WholesalerBeer> CreateAsync(WholesalerBeer wholesalerBeer);
    Task<WholesalerBeer> UpdateStockAsync(WholesalerBeer wholesalerBeer);
    Task SaveAsync();
}
