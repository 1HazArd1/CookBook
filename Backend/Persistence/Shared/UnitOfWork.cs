using iMocha.Talent.Analytics.Application.Interface.Persistence;

namespace iMocha.Talent.Analytics.Persistence.Shared
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
