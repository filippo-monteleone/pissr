namespace BusinessLogicLayer.Services;

public interface IDataService<T> where T : class
{
    Task<IEnumerable<T>> GetList();
    Task<T?> GetById(int id);
    Task<T?> Insert(T entity);
    Task<T?> Update(T entity);
    Task<T?> Delete(T entity);
}