using MaaltijdenApp_Core.Models;

namespace MaaltijdenApp_Core.services
{
    public interface IProductRepository
    {
        public Task<Product> Create(Product product);
        public IQueryable<Product> GetAll();
        public IQueryable<Product> GetByName(string name);
        public Task<Product?> GetById(Guid productId);
        public Task<Product> Update(Product product);
        public Task<Product> Delete(Guid productId);
    }
}
