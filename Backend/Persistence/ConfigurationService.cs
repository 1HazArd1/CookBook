using CookBook.Application.Interface.Persistence.Users;
using CookBook.Persistence.Shared;
using CookBook.Persistence.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CookBook.Persistence
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
