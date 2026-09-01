namespace BeerStockApi.Contracts;

public sealed record WholesalerStockResponse(
    int Id,
    string Name,
    int Quantity);