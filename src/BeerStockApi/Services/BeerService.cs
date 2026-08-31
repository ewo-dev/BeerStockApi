using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;
using BeerStockApi.Domain;
using BeerStockApi.Exceptions;
using BeerStockApi.Repositories;

namespace BeerStockApi.Services;

public class BeerService(IBeerRepository beerRepository, ILogger<BeerService> logger) : IBeerService
{
    private readonly IBeerRepository _beerRepository = beerRepository;
    private readonly ILogger<BeerService> _logger = logger;

    public async Task<List<BeerResponse>> GetAllBeersAsync()
    {
        var beers = await _beerRepository.GetAllAsync();
        return beers.Select(MapToBeerResponse).ToList();
    }

    public async Task<BeerResponse> GetBeerByIdAsync(int id)
    {
        var beer = await _beerRepository.GetByIdAsync(id) ?? throw new BusinessException($"Bière avec l'ID {id} introuvable.");
        return MapToBeerResponse(beer);
    }

    public async Task<List<BeerResponse>> GetBeersByBrewerAsync(int brewerId)
    {
        var beers = await _beerRepository.GetByBrewerIdAsync(brewerId);
        return beers.Select(MapToBeerResponse).ToList();
    }

    public async Task<BeerResponse> CreateBeerAsync(CreateBeerRequest request)
    {
        var beer = new Beer
        {
            Name = request.Name,
            AlcoholByVolume = request.AlcoholByVolume,
            UnitPriceExcludingVat = request.UnitPriceExcludingVat,
            BrewerId = request.BrewerId
        };

        var createdBeer = await _beerRepository.CreateAsync(beer);
        _logger.LogInformation("Bière créée: {BeerId} - {BeerName}", createdBeer.Id, createdBeer.Name);

        return MapToBeerResponse(createdBeer);
    }

    public async Task<BeerResponse> UpdateBeerAsync(int id, UpdateBeerRequest request)
    {
        var beer = await _beerRepository.GetByIdAsync(id) ?? throw new BusinessException($"Bière avec l'ID {id} introuvable.");

        // Mettre à jour les champs fournis
        if (!string.IsNullOrWhiteSpace(request.Name))
            beer.Name = request.Name;

        if (request.AlcoholByVolume.HasValue)
            beer.AlcoholByVolume = request.AlcoholByVolume.Value;

        if (request.UnitPriceExcludingVat.HasValue)
            beer.UnitPriceExcludingVat = request.UnitPriceExcludingVat.Value;

        if (request.BrewerId.HasValue)
            beer.BrewerId = request.BrewerId.Value;

        var updatedBeer = await _beerRepository.UpdateAsync(beer);
        _logger.LogInformation("Bière mise à jour: {BeerId}", updatedBeer.Id);

        return MapToBeerResponse(updatedBeer);
    }

    public async Task DeleteBeerAsync(int id)
    {
        var beer = await _beerRepository.GetByIdAsync(id) ?? throw new BusinessException($"Bière avec l'ID {id} introuvable.");
        await _beerRepository.DeleteAsync(id);
        _logger.LogInformation("Bière supprimée: {BeerId}", id);
    }

    private static BeerResponse MapToBeerResponse(Beer beer)
    {
        var brewerResponse = new BrewerResponse(
            beer.Brewer?.Id ?? 0,
            beer.Brewer?.Name ?? "Unknown");

        var wholesalerStocks = beer.WholesalerBeers?.Select(wb => new WholesalerStockResponse(
            wb.WholesalerId,
            wb.Wholesaler?.Name ?? "Unknown",
            wb.Quantity)).ToList() ?? [];

        return new BeerResponse(
            beer.Id,
            beer.Name,
            beer.AlcoholByVolume,
            beer.UnitPriceExcludingVat,
            brewerResponse,
            wholesalerStocks);
    }
}
