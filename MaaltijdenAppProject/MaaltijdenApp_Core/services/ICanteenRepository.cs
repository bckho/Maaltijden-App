using MaaltijdenApp_Core.Models;

namespace MaaltijdenApp_Core.services
{
    public interface ICanteenRepository
    {
        public Task<Canteen> Create(Canteen canteen);
        public IQueryable<Canteen> GetAll();
        public Task<Canteen?> GetById(Guid canteenId);
    }
}
