using MaaltijdenApp_Core.Models;

namespace MaaltijdenApp_Core.services
{
    public interface IStudentRepository
    {
        public Task<Student> Create(Student student);
        public IQueryable<Student> GetAll();
        public Task<Student?> GetById(Guid studentId);
        public Task<Student?> GetByEmail(string email);
        public Task<Student> Update(Student student);
        public Task<Student> ReportNoShow(Guid studentId);
    }
}
