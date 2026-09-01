using BeerStockApi.Domain;
using BeerStockApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeerStockApi.Repositories;

public class WholesalerRepository(BeerStockApiDbContext context) : IWholesalerRepository
{
    private readonly BeerStockApiDbContext _context = context;

    public async Task<Wholesaler?> GetByIdAsync(int id)
    {
        return await _context.Wholesalers
            .Include(w => w.WholesalerBeers)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<List<Wholesaler>> GetAllAsync()
    {
        return await _context.Wholesalers
            .Include(w => w.WholesalerBeers)
            .AsNoTracking()
            .ToListAsync();
    }
}
