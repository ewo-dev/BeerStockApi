namespace BeerStockApi.Contracts;

public sealed record BeerStockResponse(
    int BeerId,
    string BeerName,
    int Quantity);
