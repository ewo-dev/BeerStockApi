using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;
using BeerStockApi.Exceptions;
using BeerStockApi.Repositories;

namespace BeerStockApi.Services;

public class StockService(
    IWholesalerBeerRepository wholesalerBeerRepository,
    IBeerRepository beerRepository,
    IWholesalerRepository wholesalerRepository,
    ILogger<StockService> logger) : IStockService
{
    private readonly IWholesalerBeerRepository _wholesalerBeerRepository = wholesalerBeerRepository;
    private readonly IBeerRepository _beerRepository = beerRepository;
    private readonly IWholesalerRepository _wholesalerRepository = wholesalerRepository;
    private readonly ILogger<StockService> _logger = logger;

    public async Task<WholesalerStockResponse> GetStockAsync(int wholesalerId, int beerId)
    {
        var stock = await _wholesalerBeerRepository.GetStockAsync(wholesalerId, beerId) ?? throw new BusinessException($"Stock non trouvé pour le grossiste {wholesalerId} et la bière {beerId}.");
        return MapToWholesalerStockResponse(stock);
    }

    public async Task<List<BeerStockResponse>> GetStocksByWholesalerAsync(int wholesalerId)
    {
        var stocks = await _wholesalerBeerRepository.GetByWholesalerIdAsync(wholesalerId);
            return [.. stocks.Select(MapToBeerStockResponse)];
    }

    public async Task<List<WholesalerStockResponse>> GetWholesalersForBeerAsync(int beerId)
    {
        var stocks = await _wholesalerBeerRepository.GetByBeerIdAsync(beerId);
            return [.. stocks.Select(MapToWholesalerStockResponse)];
    }

    public async Task<WholesalerStockResponse> AddBeerToWholesalerAsync(int wholesalerId, CreateBeerSaleRequest request)
    {
        if (request.Quantity <= 0)
        {
            throw new ValidationException("La quantité doit être supérieure à 0.");
        }

        var wholesaler = await _wholesalerRepository.GetByIdAsync(wholesalerId) 
            ?? throw new BusinessException($"Le grossiste avec l'ID {wholesalerId} n'existe pas.");

        var beer = await _beerRepository.GetByIdAsync(request.BeerId) 
            ?? throw new BusinessException($"La bière avec l'ID {request.BeerId} n'existe pas.");

        var existingStock = await _wholesalerBeerRepository.GetStockAsync(wholesalerId, request.BeerId);
        if (existingStock != null)
        {
            throw new ValidationException($"La bière '{beer.Name}' est déjà dans le catalogue du grossiste '{wholesaler.Name}'.");
        }

        var wholesalerBeer = new Domain.WholesalerBeer
        {
            WholesalerId = wholesalerId,
            BeerId = request.BeerId,
            Quantity = request.Quantity
        };

        var createdStock = await _wholesalerBeerRepository.CreateAsync(wholesalerBeer);
        _logger.LogInformation("Bière ajoutée au grossiste: Grossiste {WholesalerId}, Bière {BeerId}, Quantité {Quantity}",
            wholesalerId, request.BeerId, request.Quantity);

        return MapToWholesalerStockResponse(createdStock);
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
    private static BeerStockResponse MapToBeerStockResponse(Domain.WholesalerBeer stock)
    {
        return new BeerStockResponse(
            stock.BeerId,
            stock.Beer?.Name ?? "Unknown",
            stock.Quantity);
    }
}
