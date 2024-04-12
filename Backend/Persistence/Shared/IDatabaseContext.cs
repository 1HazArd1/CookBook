using iMocha.Talent.Analytics.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace iMocha.Talent.Analytics.Persistence.Shared
{
    public interface IDatabaseContext
    {
        DbSet<T> Set<T>() where T : class, IEntity;

        Task<int> SaveAsync();
    }
}
