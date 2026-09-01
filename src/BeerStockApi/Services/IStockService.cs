using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;

namespace BeerStockApi.Services;

public interface IStockService
{
    Task<WholesalerStockResponse> GetStockAsync(int wholesalerId, int beerId);
    Task<List<BeerStockResponse>> GetStocksByWholesalerAsync(int wholesalerId);
    Task<List<WholesalerStockResponse>> GetWholesalersForBeerAsync(int beerId);
    Task<WholesalerStockResponse> AddBeerToWholesalerAsync(int wholesalerId, CreateBeerSaleRequest request);
    Task<WholesalerStockResponse> UpdateStockAsync(int wholesalerId, int beerId, int newQuantity);
}
