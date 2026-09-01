using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;
using BeerStockApi.Services;

namespace BeerStockApi.Endpoints;

public static class BeerEndpoints
{
    public static IEndpointRouteBuilder MapBeerEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/beers")
            .WithTags("Beers");

        group.MapGet("/", GetAllBeers)
            .WithName("GetAllBeers")
            .WithSummary("Get all beers")
            .WithDescription("Returns a list of all beers with their brewer and stock information")
            .Produces<List<BeerResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/brewers/{brewerId}", GetBeersByBrewer)
            .WithName("GetBeersByBrewer")
            .WithSummary("Get beers by brewer")
            .WithDescription("Returns a list of all beers from a specific brewer")
            .Produces<List<BeerResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetBeerById)
            .WithName("GetBeerById")
            .WithSummary("Get a beer by ID")
            .WithDescription("Returns a specific beer with its brewer and stock information at each wholesaler")
            .Produces<BeerResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", CreateBeer)
            .WithName("CreateBeer")
            .WithSummary("Create a new beer")
            .WithDescription("Creates a new beer in the system")
            .Accepts<CreateBeerRequest>("application/json")
            .Produces<BeerResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", UpdateBeer)
            .WithName("UpdateBeer")
            .WithSummary("Update a beer")
            .WithDescription("Updates an existing beer (partial updates supported)")
            .Accepts<UpdateBeerRequest>("application/json")
            .Produces<BeerResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", DeleteBeer)
            .WithName("DeleteBeer")
            .WithSummary("Delete a beer")
            .WithDescription("Deletes a beer from the system")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static async Task<IResult> GetAllBeers(IBeerService beerService)
    {
        var beers = await beerService.GetAllBeersAsync();
        return Results.Ok(beers);
    }

    private static async Task<IResult> GetBeersByBrewer(int brewerId, IBeerService beerService)
    {
        var beers = await beerService.GetBeersByBrewerAsync(brewerId);
        return Results.Ok(beers);
    }

    private static async Task<IResult> GetBeerById(int id, IBeerService beerService)
    {
        var beer = await beerService.GetBeerByIdAsync(id);
        return Results.Ok(beer);
    }

    private static async Task<IResult> CreateBeer(CreateBeerRequest request, IBeerService beerService)
    {
        var beer = await beerService.CreateBeerAsync(request);
        return Results.Created($"/api/beers/{beer.Id}", beer);
    }

    private static async Task<IResult> UpdateBeer(int id, UpdateBeerRequest request, IBeerService beerService)
    {
        var beer = await beerService.UpdateBeerAsync(id, request);
        return Results.Ok(beer);
    }

    private static async Task<IResult> DeleteBeer(int id, IBeerService beerService)
    {
        await beerService.DeleteBeerAsync(id);
        return Results.NoContent();
    }
}
