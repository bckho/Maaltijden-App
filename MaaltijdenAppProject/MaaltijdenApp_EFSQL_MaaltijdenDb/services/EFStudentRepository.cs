using MaaltijdenApp_Core.Models;
using MaaltijdenApp_Core.services;
using Microsoft.EntityFrameworkCore;

namespace MaaltijdenApp_EFSQL_MaaltijdenDb.services
{
    public class EFStudentRepository : IStudentRepository
    {
        private readonly MaaltijdenAppDbContext _dbContext;

        public EFStudentRepository(MaaltijdenAppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Student> Create(Student student)
        {
            var result = await _dbContext.Students.FirstOrDefaultAsync(s => s.Email.Equals(student.Email));

            if (result != null) throw new Exception("Student with email already exists.");

            // Check if student's age is atleast 16.
            if (!student.AgeIsValid()) throw new Exception("Student is nog geen 16 jaar oud.");

            var created = await _dbContext.Students.AddAsync(student);

            await _dbContext.SaveChangesAsync();

            return created.Entity;
        }

        public IQueryable<Student> GetAll() =>
            _dbContext.Students;

        public async Task<Student?> GetById(Guid studentId) =>
            await _dbContext.Students.FirstOrDefaultAsync(s => s.Id.Equals(studentId));

        public async Task<Student?> GetByEmail(string email) =>
            await _dbContext.Students.FirstOrDefaultAsync(s => s.Email.Equals(email));

        public async Task<Student> ReportNoShow(Guid studentId)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id.Equals(studentId));

            if (student == null) throw new Exception("Student niet gevonden.");

            student.NoShowCount++;

            var updated = _dbContext.Update(student);

            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<Student> Update(Student student)
        {
            var result = _dbContext.Students.Update(student);

            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }
}
