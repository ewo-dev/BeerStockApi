using System.ComponentModel.DataAnnotations;

namespace BeerStockApi.Domain;
public class Brewer
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }

    public ICollection<Beer> Beers { get; set; } = [];
}