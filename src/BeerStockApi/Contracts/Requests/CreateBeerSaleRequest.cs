namespace BeerStockApi.Contracts.Requests;

public record CreateBeerSaleRequest
{
    public required int BeerId { get; init; }
    public required int Quantity { get; init; }
}
