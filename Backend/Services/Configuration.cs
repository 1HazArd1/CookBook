using CookBook.Application.Interface.Auth;
using CookBook.Services.Filters;
using CookBook.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CookBook.Services
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

            services.SetAuthentication(configuration);

            services.AddScoped(typeof(ITokenService), typeof(JWTTokenService));
            services.AddScoped(typeof(ICurrentUserService), typeof(CurrentUserService));
            return services;
        }
        private static void SetCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsConfig = new CorsConfig();
            configuration.GetSection(CorsConfig.CORS).Bind(corsConfig);
            var origins = corsConfig.Origins ?? new string[] { "*" };

            services.AddCors(p =>
               p.AddPolicy("CorsPolicy", build => {
                   build.WithOrigins(origins).AllowAnyHeader().WithMethods("GET", "POST" , "PATCH" , "DELETE").SetPreflightMaxAge(TimeSpan.FromMinutes(20));
               }
           ));
        }
        public static void SetAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });
            services.AddAuthorization();
        }
    }
    public class CorsConfig
    {
        public const string CORS = "CORS";

        public string[]? Origins { get; set; }
    }

}
