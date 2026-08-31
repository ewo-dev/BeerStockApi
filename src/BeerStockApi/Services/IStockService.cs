using BeerStockApi.Contracts;

namespace BeerStockApi.Services;

public interface IStockService
{
    Task<WholesalerStockResponse> GetStockAsync(int wholesalerId, int beerId);
    Task<List<WholesalerStockResponse>> GetStocksByWholesalerAsync(int wholesalerId);
    Task<WholesalerStockResponse> UpdateStockAsync(int wholesalerId, int beerId, int newQuantity);
}
