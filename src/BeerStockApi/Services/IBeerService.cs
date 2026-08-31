using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;

namespace BeerStockApi.Services;

public interface IBeerService
{
    Task<List<BeerResponse>> GetAllBeersAsync();
    Task<BeerResponse> GetBeerByIdAsync(int id);
    Task<List<BeerResponse>> GetBeersByBrewerAsync(int brewerId);
    Task<BeerResponse> CreateBeerAsync(CreateBeerRequest request);
    Task<BeerResponse> UpdateBeerAsync(int id, UpdateBeerRequest request);
    Task DeleteBeerAsync(int id);
}
