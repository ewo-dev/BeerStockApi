using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;
using BeerStockApi.Services;

namespace BeerStockApi.Endpoints;

public static class QuoteEndpoints
{
    public static IEndpointRouteBuilder MapQuoteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/quotes")
            .WithTags("Quotes");

        group.MapPost("/", CreateQuote)
            .WithName("CreateQuote")
            .WithSummary("Create a quote")
            .WithDescription("Creates a quote for a wholesaler order with quantity discounts and VAT")
            .Accepts<CreateQuoteRequest>("application/json")
            .Produces<QuoteResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static async Task<IResult> CreateQuote(CreateQuoteRequest request, IQuoteService quoteService)
    {
        try
        {
            var quote = await quoteService.GenerateQuoteAsync(request);
            return Results.Ok(quote);
        }
        catch (Exception ex) when (ex.GetType().Name == "ValidationException")
        {
            return Results.UnprocessableEntity(new { message = ex.Message });
        }
        catch (Exception ex) when (ex.GetType().Name == "BusinessException")
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }
}
