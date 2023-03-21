using CachingWithRedisDemo.Data;
using Microsoft.AspNet.OData;
using Microsoft.EntityFrameworkCore;

namespace CachingWithRedisDemo.Services
{
    public class StudentService<T> : IStudentService<T> where T : class
    {
        public DataContext _context;
        public DbSet<T> _dbSet;
        public StudentService(DataContext context) 
        { 
            _context = context;
            _dbSet = _context.Set<T>();
        }
        [EnableQuery()]
        public async Task<List<T>> GetAll()
        {
            var result = await _dbSet.ToListAsync();
            if (result == null) return null;
            return result;
        }
        [EnableQuery()]
        public async Task<T> GetById(int id)
        {
            var result = await _dbSet.FindAsync(id);
            return result;
        }

        public async Task AddStudent(T student)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
        }

        public Task DeleteStudent(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStudent(T student)
        {
            throw new NotImplementedException();
        }
    }
}
