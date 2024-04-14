using CookBook.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Persistence.Shared
{
    public interface IDatabaseContext
    {
        DbSet<T> Set<T>() where T : class, IEntity;

        Task<int> SaveAsync();
    }
}
