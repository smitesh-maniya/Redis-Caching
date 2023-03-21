namespace CachingWithRedisDemo.Services
{
    public interface IStudentService<T>
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task AddStudent(T student);
        Task UpdateStudent(T student);
        Task DeleteStudent(int id);
    }
}