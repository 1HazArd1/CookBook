using iMocha.Talent.Analytics.Application.Interface.Persistence.Users;
using iMocha.Talent.Analytics.Persistence.Shared;
using iMocha.Talent.Analytics.Persistence.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iMocha.Talent.Analytics.Persistence
{
    public static class ConfigurationService
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("iMochaDBConnection")), ServiceLifetime.Scoped);

            services.AddScoped(typeof(IDatabaseContext), typeof(DatabaseContext));
            services.AddScoped(typeof(IUserMasterRepository), typeof(UserMasterRepository));
            return services;
        }
    }
}
