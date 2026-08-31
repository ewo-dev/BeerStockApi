using System.ComponentModel.DataAnnotations;

namespace BeerStockApi.Domain;

public class Wholesaler
{
    public int Id { get; set; }
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }

    public ICollection<WholesalerBeer> WholesalerBeers { get; set; } = [];
}