namespace BeerStockApi.Contracts.Requests;

public record UpdateBeerRequest
{
    public string? Name { get; init; }
    public decimal? AlcoholByVolume { get; init; }
    public decimal? UnitPriceExcludingVat { get; init; }
    public int? BrewerId { get; init; }
}
