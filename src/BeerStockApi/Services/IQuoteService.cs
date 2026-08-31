using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;

namespace BeerStockApi.Services;

public interface IQuoteService
{
    Task<QuoteResponse> GenerateQuoteAsync(CreateQuoteRequest request);
}
