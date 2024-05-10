using CookBook.Application.Interface.Persistence;

namespace CookBook.Persistence.Shared
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDatabaseContext database;

        public UnitOfWork(IDatabaseContext database)
        {
            this.database = database;
        }

        public async Task<int> SaveAsync()
        {
            return await database.SaveAsync();
        }
    }
}
