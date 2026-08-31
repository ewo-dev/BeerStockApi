namespace BeerStockApi.Contracts.Requests;
public record CreateBeerRequest
{
    public required string Name { get; init; }
    public required decimal AlcoholByVolume { get; init; }
    public required decimal UnitPriceExcludingVat { get; init; }
    public required int BrewerId { get; init; }
}
