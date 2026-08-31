namespace BeerStockApi.Contracts.Requests;
public record CreateQuoteRequest
{
    public required int WholesalerId { get; init; }

    public required List<QuoteLineRequest> Lines { get; init; }
}

public record QuoteLineRequest
{
    public required int BeerId { get; init; }

    public required int Quantity { get; init; }
}
