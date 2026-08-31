using BeerStockApi.Domain;
using BeerStockApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeerStockApi.Repositories;

public class BeerRepository(BeerStockApiDbContext context) : IBeerRepository
{
    private readonly BeerStockApiDbContext _context = context;

    public async Task<List<Beer>> GetAllAsync()
    {
        return await _context.Beers
            .Include(b => b.Brewer)
            .Include(b => b.WholesalerBeers)
            .ThenInclude(wb => wb.Wholesaler)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Beer?> GetByIdAsync(int id)
    {
        return await _context.Beers
            .Include(b => b.Brewer)
            .Include(b => b.WholesalerBeers)
            .ThenInclude(wb => wb.Wholesaler)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Beer>> GetByBrewerIdAsync(int brewerId)
    {
        return await _context.Beers
            .Where(b => b.BrewerId == brewerId)
            .Include(b => b.Brewer)
            .Include(b => b.WholesalerBeers)
            .ThenInclude(wb => wb.Wholesaler)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Beer> CreateAsync(Beer beer)
    {
        _context.Beers.Add(beer);
        await _context.SaveChangesAsync();
        return beer;
    }

    public async Task<Beer> UpdateAsync(Beer beer)
    {
        _context.Beers.Update(beer);
        await _context.SaveChangesAsync();
        return beer;
    }

    public async Task DeleteAsync(int id)
    {
        var beer = await _context.Beers.FindAsync(id);
        if (beer != null)
        {
            _context.Beers.Remove(beer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
