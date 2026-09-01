using BeerStockApi.Domain;
using BeerStockApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeerStockApi.Repositories;

public class WholesalerBeerRepository(BeerStockApiDbContext context) : IWholesalerBeerRepository
{
    private readonly BeerStockApiDbContext _context = context;

    public async Task<WholesalerBeer?> GetStockAsync(int wholesalerId, int beerId)
    {
        return await _context.WholesalerBeers
            .Include(wb => wb.Beer)
            .Include(wb => wb.Wholesaler)
            .FirstOrDefaultAsync(wb => wb.WholesalerId == wholesalerId && wb.BeerId == beerId);
    }

    public async Task<List<WholesalerBeer>> GetByWholesalerIdAsync(int wholesalerId)
    {
        return await _context.WholesalerBeers
            .Where(wb => wb.WholesalerId == wholesalerId)
            .Include(wb => wb.Beer)
            .Include(wb => wb.Wholesaler)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<WholesalerBeer>> GetByBeerIdAsync(int beerId)
    {
        return await _context.WholesalerBeers
            .Where(wb => wb.BeerId == beerId)
            .Include(wb => wb.Beer)
            .Include(wb => wb.Wholesaler)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<WholesalerBeer> CreateAsync(WholesalerBeer wholesalerBeer)
    {
        _context.WholesalerBeers.Add(wholesalerBeer);
        await _context.SaveChangesAsync();
        return await GetStockAsync(wholesalerBeer.WholesalerId, wholesalerBeer.BeerId) ?? wholesalerBeer;
    }

    public async Task<WholesalerBeer> UpdateStockAsync(WholesalerBeer wholesalerBeer)
    {
        _context.WholesalerBeers.Update(wholesalerBeer);
        await _context.SaveChangesAsync();
        return wholesalerBeer;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
