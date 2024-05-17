using MaaltijdenApp_Core.Models;
using MaaltijdenApp_Core.services;
using Microsoft.EntityFrameworkCore;

namespace MaaltijdenApp_EFSQL_MaaltijdenDb.services
{
    public class EFEmployeeRepository : IEmployeeRepository
    {
        private readonly MaaltijdenAppDbContext _dbContext;

        public EFEmployeeRepository(MaaltijdenAppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Employee> Create(Employee employee)
        {
            var result = await _dbContext.Employees.AddAsync(employee);

            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<Employee?> GetById(Guid employeeId) => 
            await _dbContext.Employees.Include(e => e.Canteen)
            .FirstOrDefaultAsync(e => e.Id.Equals(employeeId));

        public async Task<Employee?> GetByEmail(string email) =>
            await _dbContext.Employees.Include(e => e.Canteen)
            .FirstOrDefaultAsync(e => e.Email.Equals(email));

        public async Task<Employee> Update(Employee employee)
        {
            var result = _dbContext.Employees.Update(employee);

            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }
}
