using BeerStockApi.Domain;

namespace BeerStockApi.Repositories;

public interface IWholesalerRepository
{
    Task<Wholesaler?> GetByIdAsync(int id);
    Task<List<Wholesaler>> GetAllAsync();
}
