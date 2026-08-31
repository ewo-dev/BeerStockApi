using BeerStockApi.Contracts;
using BeerStockApi.Exceptions;
using BeerStockApi.Repositories;

namespace BeerStockApi.Services;

public class StockService(IWholesalerBeerRepository wholesalerBeerRepository, ILogger<StockService> logger) : IStockService
{
    private readonly IWholesalerBeerRepository _wholesalerBeerRepository = wholesalerBeerRepository;
    private readonly ILogger<StockService> _logger = logger;

    public async Task<WholesalerStockResponse> GetStockAsync(int wholesalerId, int beerId)
    {
        var stock = await _wholesalerBeerRepository.GetStockAsync(wholesalerId, beerId) ?? throw new BusinessException($"Stock non trouvé pour le grossiste {wholesalerId} et la bière {beerId}.");
        return MapToWholesalerStockResponse(stock);
    }

    public async Task<List<WholesalerStockResponse>> GetStocksByWholesalerAsync(int wholesalerId)
    {
        var stocks = await _wholesalerBeerRepository.GetByWholesalerIdAsync(wholesalerId);
        return stocks.Select(MapToWholesalerStockResponse).ToList();
    }

    public async Task<WholesalerStockResponse> UpdateStockAsync(int wholesalerId, int beerId, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new ValidationException("La quantité ne peut pas être négative.");
        }

        var stock = await _wholesalerBeerRepository.GetStockAsync(wholesalerId, beerId) ?? throw new BusinessException($"Stock non trouvé pour le grossiste {wholesalerId} et la bière {beerId}.");
        stock.Quantity = newQuantity;
        var updatedStock = await _wholesalerBeerRepository.UpdateStockAsync(stock);
        _logger.LogInformation("Stock mis à jour: Grossiste {WholesalerId}, Bière {BeerId}, Nouvelle quantité {Quantity}",
            wholesalerId, beerId, newQuantity);

        return MapToWholesalerStockResponse(updatedStock);
    }

    private static WholesalerStockResponse MapToWholesalerStockResponse(Domain.WholesalerBeer stock)
    {
        return new WholesalerStockResponse(
            stock.WholesalerId,
            stock.Wholesaler?.Name ?? "Unknown",
            stock.Quantity);
    }
}
