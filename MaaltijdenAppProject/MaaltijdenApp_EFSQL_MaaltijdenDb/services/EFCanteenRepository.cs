using MaaltijdenApp_Core.Models;
using MaaltijdenApp_Core.services;
using Microsoft.EntityFrameworkCore;

namespace MaaltijdenApp_EFSQL_MaaltijdenDb.services
{
    public class EFCanteenRepository : ICanteenRepository
    {
        private readonly MaaltijdenAppDbContext _dbContext;

        public EFCanteenRepository(MaaltijdenAppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Canteen> Create(Canteen canteen)
        {
            var result = await _dbContext.Canteens.AddAsync(canteen);

            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }

        public IQueryable<Canteen> GetAll() =>
            _dbContext.Canteens;

        public async Task<Canteen?> GetById(Guid canteenId) => await _dbContext.Canteens.FirstOrDefaultAsync(c => c.Id.Equals(canteenId));
    }
}
