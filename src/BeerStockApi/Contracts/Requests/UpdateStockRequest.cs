namespace BeerStockApi.Contracts.Requests;

public record UpdateStockRequest
{
    public int Quantity { get; init; }
}
