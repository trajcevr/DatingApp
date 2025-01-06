using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Add Token Service
            services.AddScoped<ITokenService, TokenService>();

            // Add DbContext
            services.AddDbContext<API.Data.DataContext>(options =>
                options.UseSqlite(config.GetConnectionString("DefaultConnection"))
            );

            return services;
        }
    }
}
