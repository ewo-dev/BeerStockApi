using BeerStockApi.Contracts;

namespace BeerStockApi.Services;

public interface IStockService
{
    Task<WholesalerStockResponse> GetStockAsync(int wholesalerId, int beerId);
    Task<List<BeerStockResponse>> GetStocksByWholesalerAsync(int wholesalerId);
    Task<List<WholesalerStockResponse>> GetWholesalersForBeerAsync(int beerId);
    Task<WholesalerStockResponse> UpdateStockAsync(int wholesalerId, int beerId, int newQuantity);
}
