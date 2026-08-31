namespace BeerStockApi.Contracts;

public record QuoteResponse
{
    public required List<QuoteLineResponse> Lines { get; init; }
    public required decimal SubTotal { get; init; }
    public required decimal VAT { get; init; }
    public required decimal FinalTotal { get; init; }
}

public record QuoteLineResponse
{
    public required int BeerId { get; init; }
    public required string BeerName { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public required decimal DiscountPercent { get; init; }
    public required decimal LineTotal { get; init; }
}
