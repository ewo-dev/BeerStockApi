using BeerStockApi.Contracts;
using BeerStockApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeerStockApi.Endpoints;

    public static class BeerEndpoints
    {
        public static IEndpointRouteBuilder MapBeerEndpoints(
            this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/beers")
                .WithTags("Beers");

            group.MapGet("/", async (BeerStockApiDbContext dbContext) =>
            {
                var beers = await dbContext.Beers
                    .AsNoTracking()
                    .Include(beer => beer.Brewer)
                    .Include(beer => beer.WholesalerBeers)
                        .ThenInclude(stock => stock.Wholesaler)
                    .Select(beer => new BeerResponse(
                        beer.Id,
                        beer.Name,
                        beer.AlcoholByVolume,
                        beer.UnitPriceExcludingVat,
                        new BrewerResponse(beer.Brewer.Id, beer.Brewer.Name),
                        beer.WholesalerBeers.Select(stock => new WholesalerStockResponse(
                            stock.Wholesaler.Id,
                            stock.Wholesaler.Name,
                            stock.Quantity)).ToList()))
                    .ToListAsync();

                return Results.Ok(beers);
            })
            .WithName("GetBeers")
            .WithSummary("Lists beers with their brewer and wholesaler stock")
            .Produces<List<BeerResponse>>();

            return app;
        }
    }
