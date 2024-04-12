using iMocha.Talent.Analytics.Services.Filters;

namespace iMocha.Talent.Analytics.Services
{
    public static class ConfigurationService
    {
        public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            });

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.SetCors(configuration);
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            return services;
        }
        private static void SetCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsConfig = new CorsConfig();
            configuration.GetSection(CorsConfig.CORS).Bind(corsConfig);
            var origins = corsConfig.Origins ?? new string[] { "*" };

            services.AddCors(p =>
               p.AddPolicy("CorsPolicy", build => {
                   build.WithOrigins(origins).AllowAnyHeader().WithMethods("GET", "POST").SetPreflightMaxAge(TimeSpan.FromMinutes(20));
               }
           ));
        }
    }
    public class CorsConfig
    {
        public const string CORS = "CORS";

        public string[]? Origins { get; set; }
    }

}
