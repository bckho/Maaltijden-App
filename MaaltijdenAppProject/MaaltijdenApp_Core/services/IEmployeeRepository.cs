using MaaltijdenApp_Core.Models;

namespace MaaltijdenApp_Core.services
{
    public interface IEmployeeRepository
    {
        public Task<Employee> Create(Employee employee);
        public Task<Employee?> GetById(Guid employeeId);
        public Task<Employee?> GetByEmail(string email);
        public Task<Employee> Update(Employee employee);
    }
}
