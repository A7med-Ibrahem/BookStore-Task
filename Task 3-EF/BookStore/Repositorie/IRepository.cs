namespace BookStoreApp.Repositories
{
    public interface IRepository<T>
    {
        void Add(T entity);
        void Remove(int id);
        T? GetById(int id);
        List<T> GetAll();
    }
}