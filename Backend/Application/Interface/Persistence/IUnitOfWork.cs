namespace CookBook.Application.Interface.Persistence
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
    }
}
