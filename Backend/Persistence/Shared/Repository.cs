using iMocha.Talent.Analytics.Application.Interface.Persistence;
using iMocha.Talent.Analytics.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace iMocha.Talent.Analytics.Persistence.Shared
{
    public class Repository<T> : IRepository<T>
        where T : class, IEntity

    {
        private readonly IDatabaseContext database;

        public Repository(IDatabaseContext database)
        {
            this.database = database;
        }

        public IQueryable<T> GetAll()
        {
            return database.Set<T>();
        }

        public IQueryable<T> GetAllAsNoTracking()
        {
            return GetAll().AsNoTracking();
        }

        //public T Get(int id)
        //{
        //    return _database.Set<T>()
        //        .Single(p => p.Id == id);
        //}

        public async Task AddAsync(T entity)
        {
            await database.Set<T>().AddAsync(entity);
        }

        public void Remove(T entity)
        {
            database.Set<T>().Remove(entity);
        }
    }
}
