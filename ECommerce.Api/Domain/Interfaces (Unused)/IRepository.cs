namespace ECommerce.Api.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T>Add(T entity);
    Task<T?>Update(int id, T entity);
    Task<T?> Delete(int id);
    Task<T?> GetById(int id);
    Task<IEnumerable<T>> GetAll();
}