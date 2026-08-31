
using BeerStockApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BeerStockApi.Infrastructure;

public sealed class BeerStockApiDbContext(DbContextOptions<BeerStockApiDbContext> options) : DbContext(options)
{
    public DbSet<Beer> Beers { get; set; } 

    public DbSet<Brewer> Brewers { get; set; }

    public DbSet<Wholesaler> Wholesalers { get; set; }
    public DbSet<WholesalerBeer> WholesalerBeers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<WholesalerBeer>()
        .HasKey(wholesalerBeer => new
        {
            wholesalerBeer.WholesalerId,
            wholesalerBeer.BeerId
        });

    base.OnModelCreating(modelBuilder);
}


}