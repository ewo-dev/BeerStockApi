using System.ComponentModel.DataAnnotations;

namespace BeerStockApi.Domain;
public class Beer
{
    public int Id { get; set; }

    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }

    [Range(typeof(decimal), "0.01", "100")]
    public  decimal AlcoholByVolume { get; set; }

    [Range(typeof(decimal), "0.01", "10000")]
    public  decimal UnitPriceExcludingVat { get; set; }

    public int BrewerId { get; set; }

    public Brewer Brewer { get; set; } = null!;

    public ICollection<WholesalerBeer> WholesalerBeers { get; set; } = [];

}
