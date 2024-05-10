namespace CookBook.Application.Interface.Persistence
{
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();

        IQueryable<T> GetAllAsNoTracking();

        //T Get(int id);

        Task AddAsync(T entity);

        void Remove(T entity);
    }
}
