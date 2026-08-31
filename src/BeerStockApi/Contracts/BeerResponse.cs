namespace BeerStockApi.Contracts;

    public sealed record BeerResponse(
        int Id,
        string Name,
        decimal AlcoholByVolume,
        decimal UnitPriceExcludingVat,
        BrewerResponse Brewer,
        IReadOnlyList<WholesalerStockResponse> WholesalerStocks);
