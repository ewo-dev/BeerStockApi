using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;
using BeerStockApi.Services;

namespace BeerStockApi.Endpoints;

public static class StockEndpoints
{
    public static IEndpointRouteBuilder MapStockEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Stock Management");

        group.MapGet("/wholesalers/{wholesalerId}/beers", GetStocksByWholesaler)
            .WithName("GetStocksByWholesaler")
            .WithSummary("Get beers in stock for a wholesaler")
            .WithDescription("Returns a list of all beers available at a specific wholesaler with their quantities")
            .Produces<List<BeerStockResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/wholesalers/{wholesalerId}/beers", AddBeerToWholesaler)
            .WithName("AddBeerToWholesaler")
            .WithSummary("Add a beer to wholesaler catalog")
            .WithDescription("Adds an existing beer to a wholesaler's catalog with an initial quantity")
            .Accepts<CreateBeerSaleRequest>("application/json")
            .Produces<WholesalerStockResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/beers/{beerId}/wholesalers", GetWholesalersForBeer)
            .WithName("GetWholesalersForBeer")
            .WithSummary("Get wholesalers that sell a beer")
            .WithDescription("Returns a list of all wholesalers that have a specific beer in stock")
            .Produces<List<WholesalerStockResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/wholesalers/{wholesalerId}/beers/{beerId}/stock", UpdateStock)
            .WithName("UpdateStock")
            .WithSummary("Update stock quantity")
            .WithDescription("Updates the quantity of a beer in stock for a specific wholesaler")
            .Accepts<UpdateStockRequest>("application/json")
            .Produces<WholesalerStockResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static async Task<IResult> GetStocksByWholesaler(
        int wholesalerId,
        IStockService stockService)
    {
        var stocks = await stockService.GetStocksByWholesalerAsync(wholesalerId);
        return Results.Ok(stocks);
    }

    private static async Task<IResult> AddBeerToWholesaler(
        int wholesalerId,
        CreateBeerSaleRequest request,
        IStockService stockService)
    {
        var stock = await stockService.AddBeerToWholesalerAsync(wholesalerId, request);
        return Results.Created($"/api/wholesalers/{wholesalerId}/beers/{request.BeerId}", stock);
    }

    private static async Task<IResult> GetWholesalersForBeer(
        int beerId,
        IStockService stockService)
    {
        var wholesalers = await stockService.GetWholesalersForBeerAsync(beerId);
        return Results.Ok(wholesalers);
    }

    private static async Task<IResult> UpdateStock(
        int wholesalerId,
        int beerId,
        UpdateStockRequest request,
        IStockService stockService)
    {
        var updatedStock = await stockService.UpdateStockAsync(wholesalerId, beerId, request.Quantity);
        return Results.Ok(updatedStock);
    }
}
