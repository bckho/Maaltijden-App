using MaaltijdenApp_Core.Models;
using MaaltijdenApp_Core.services;
using Microsoft.EntityFrameworkCore;

namespace MaaltijdenApp_EFSQL_MaaltijdenDb.services
{
    public class EFProductRepository : IProductRepository
    {
        private readonly MaaltijdenAppDbContext _dbContext;

        public EFProductRepository(MaaltijdenAppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Product> Create(Product product)
        {
            var result = await _dbContext.Products.AddAsync(product);

            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<Product> Delete(Guid productId)
        {
            var result = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId) ?? throw new Exception("Product niet gevonden.");

            var deleted = _dbContext.Remove(result);

            return deleted.Entity;
        }

        public IQueryable<Product> GetAll() =>
            _dbContext.Products;

        public async Task<Product?> GetById(Guid productId) =>
            await _dbContext.Products.FirstOrDefaultAsync(p => p.Id.Equals(productId));

        public IQueryable<Product> GetByName(string name) =>
            _dbContext.Products.Where(p => p.Name.Contains(name));

        public async Task<Product> Update(Product product)
        {
            var result = _dbContext.Products.Update(product);

            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }
}
