using iMocha.Talent.Analytics.Domain.Common;
using iMocha.Talent.Analytics.Persistence.Users;
using Microsoft.EntityFrameworkCore;

namespace iMocha.Talent.Analytics.Persistence.Shared
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public new DbSet<T> Set<T>() where T : class, IEntity
        {
            return base.Set<T>();
        }

        public async Task<int> SaveAsync()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserMasterConfiguration());
        }
    }
}
