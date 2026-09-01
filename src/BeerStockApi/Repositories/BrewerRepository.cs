using BeerStockApi.Domain;
using BeerStockApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeerStockApi.Repositories;

public class BrewerRepository(BeerStockApiDbContext context) : IBrewerRepository
{
    private readonly BeerStockApiDbContext _context = context;

    public async Task<Brewer?> GetByIdAsync(int id)
    {
        return await _context.Brewers.FirstOrDefaultAsync(b => b.Id == id);
    }
}
