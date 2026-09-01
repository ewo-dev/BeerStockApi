using BeerStockApi.Domain;

namespace BeerStockApi.Repositories;

public interface IBrewerRepository
{
    Task<Brewer?> GetByIdAsync(int id);
}
