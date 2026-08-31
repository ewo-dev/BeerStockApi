using BeerStockApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BeerStockApi.Infrastructure;

public static class DbInitializer
{
    public static async Task InitializeAsync(BeerStockApiDbContext dbContext)
    {
        await dbContext.Database.MigrateAsync();

        if (await dbContext.Beers.AnyAsync())
        {
            return;
        }

        var abbayeDeLeffe = new Brewer { Name = "Abbaye de Leffe" };
        var duvelMoortgat = new Brewer { Name = "Brasserie Duvel Moortgat" };
        var chimay = new Brewer { Name = "Brasserie Chimay" };
        var orval = new Brewer { Name = "Brasserie Orval" };
        var rochefort = new Brewer { Name = "Brasserie Rochefort" };
        var achouffe = new Brewer { Name = "Brasserie d'Achouffe" };
        var koningshoeven = new Brewer { Name = "Brasserie de Koningshoeven" };

        var geneDrinks = new Wholesaler { Name = "GeneDrinks" };
        var belgianBeerSupply = new Wholesaler { Name = "Belgian Beer Supply" };
        var ardennesDrinks = new Wholesaler { Name = "Ardennes Drinks" };

        var leffeBlonde = new Beer { Name = "Leffe Blonde", AlcoholByVolume = 6.6m, UnitPriceExcludingVat = 2.20m, Brewer = abbayeDeLeffe };
        var leffeBrune = new Beer { Name = "Leffe Brune", AlcoholByVolume = 6.5m, UnitPriceExcludingVat = 2.30m, Brewer = abbayeDeLeffe };
        var duvel = new Beer { Name = "Duvel", AlcoholByVolume = 8.5m, UnitPriceExcludingVat = 2.80m, Brewer = duvelMoortgat };
        var vedettExtraBlond = new Beer { Name = "Vedett Extra Blond", AlcoholByVolume = 5.2m, UnitPriceExcludingVat = 1.90m, Brewer = duvelMoortgat };
        var chimayBleue = new Beer { Name = "Chimay Bleue", AlcoholByVolume = 9m, UnitPriceExcludingVat = 3.10m, Brewer = chimay };
        var chimayBlanche = new Beer { Name = "Chimay Blanche", AlcoholByVolume = 8m, UnitPriceExcludingVat = 2.90m, Brewer = chimay };
        var orvalBeer = new Beer { Name = "Orval", AlcoholByVolume = 6.2m, UnitPriceExcludingVat = 2.70m, Brewer = orval };
        var rochefort10 = new Beer { Name = "Rochefort 10", AlcoholByVolume = 11.2m, UnitPriceExcludingVat = 3.40m, Brewer = rochefort };
        var laChouffe = new Beer { Name = "La Chouffe", AlcoholByVolume = 8m, UnitPriceExcludingVat = 2.75m, Brewer = achouffe };
        var tripelKarmeliet = new Beer { Name = "Tripel Karmeliet", AlcoholByVolume = 8.4m, UnitPriceExcludingVat = 2.60m, Brewer = koningshoeven };

        dbContext.WholesalerBeers.AddRange(
            new WholesalerBeer { Wholesaler = geneDrinks, Beer = leffeBlonde, Quantity = 10 },
            new WholesalerBeer { Wholesaler = belgianBeerSupply, Beer = leffeBrune, Quantity = 8 },
            new WholesalerBeer { Wholesaler = geneDrinks, Beer = duvel, Quantity = 15 },
            new WholesalerBeer { Wholesaler = belgianBeerSupply, Beer = vedettExtraBlond, Quantity = 20 },
            new WholesalerBeer { Wholesaler = ardennesDrinks, Beer = chimayBleue, Quantity = 6 },
            new WholesalerBeer { Wholesaler = ardennesDrinks, Beer = chimayBlanche, Quantity = 9 },
            new WholesalerBeer { Wholesaler = geneDrinks, Beer = orvalBeer, Quantity = 12 },
            new WholesalerBeer { Wholesaler = belgianBeerSupply, Beer = rochefort10, Quantity = 5 },
            new WholesalerBeer { Wholesaler = ardennesDrinks, Beer = laChouffe, Quantity = 14 },
            new WholesalerBeer { Wholesaler = geneDrinks, Beer = tripelKarmeliet, Quantity = 11 });

        await dbContext.SaveChangesAsync();
    }
}