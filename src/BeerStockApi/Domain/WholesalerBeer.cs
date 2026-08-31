using System.ComponentModel.DataAnnotations;

namespace BeerStockApi.Domain;

public class WholesalerBeer
{
    public int WholesalerId { get; set; }
    public Wholesaler Wholesaler { get; set; } = null!;
    public int BeerId { get; set; }
    public Beer Beer { get; set; } = null!;

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }


}