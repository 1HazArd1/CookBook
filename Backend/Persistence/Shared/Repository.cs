using CookBook.Application.Interface.Persistence;
using CookBook.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CookBook.Persistence.Shared
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

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await database.Set<T>().AddRangeAsync(entities);
        }
    }
}
