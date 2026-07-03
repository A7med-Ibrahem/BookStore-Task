namespace BookStoreApp.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _items = new List<T>();
        private readonly Func<T, int> _getId;

        public Repository(Func<T, int> getId)
        {
            _getId = getId;
        }

        public void Add(T entity)
        {
            _items.Add(entity);
        }

        public void Remove(int id)
        {
            var item = GetById(id);
            if (item != null)
                _items.Remove(item);
        }

        public T? GetById(int id)
        {
            return _items.FirstOrDefault(i => _getId(i) == id);
        }

        public List<T> GetAll()
        {
            return _items.ToList();
        }
    }
}