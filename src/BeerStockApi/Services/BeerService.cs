using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;
using BeerStockApi.Domain;
using BeerStockApi.Exceptions;
using BeerStockApi.Repositories;

namespace BeerStockApi.Services;

public class BeerService(IBeerRepository beerRepository, IBrewerRepository brewerRepository, ILogger<BeerService> logger) : IBeerService
{
    private readonly IBeerRepository _beerRepository = beerRepository;
    private readonly IBrewerRepository _brewerRepository = brewerRepository;
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
        ValidateName(request.Name);
        ValidateAlcoholByVolume(request.AlcoholByVolume);
        ValidateUnitPrice(request.UnitPriceExcludingVat);

        _ = await _brewerRepository.GetByIdAsync(request.BrewerId)
            ?? throw new BusinessException($"Le brasseur avec l'ID {request.BrewerId} n'existe pas.");

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

        if (request.Name != null)
        {
            ValidateName(request.Name);
            beer.Name = request.Name;
        }

        if (request.AlcoholByVolume.HasValue)
        {
            ValidateAlcoholByVolume(request.AlcoholByVolume.Value);
            beer.AlcoholByVolume = request.AlcoholByVolume.Value;
        }

        if (request.UnitPriceExcludingVat.HasValue)
        {
            ValidateUnitPrice(request.UnitPriceExcludingVat.Value);
            beer.UnitPriceExcludingVat = request.UnitPriceExcludingVat.Value;
        }

        if (request.BrewerId.HasValue)
        {
            _ = await _brewerRepository.GetByIdAsync(request.BrewerId.Value)
                ?? throw new BusinessException($"Le brasseur avec l'ID {request.BrewerId.Value} n'existe pas.");
            beer.BrewerId = request.BrewerId.Value;
        }

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

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Le nom de la bière est obligatoire.");
        }
        if (name.Length > 100)
        {
            throw new ValidationException("Le nom de la bière ne peut pas dépasser 100 caractères.");
        }
    }

    private static void ValidateAlcoholByVolume(decimal alcoholByVolume)
    {
        if (alcoholByVolume <= 0 || alcoholByVolume > 100)
        {
            throw new ValidationException("Le taux d'alcool doit être compris entre 0 et 100.");
        }
    }

    private static void ValidateUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
        {
            throw new ValidationException("Le prix unitaire doit être supérieur à 0.");
        }
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
