using ACS_Reception.Persistence.UnitOfWork;
using ACS_Reception.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ACS_Reception.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddSingleton<IUnitOfWork, EfUnitOfWork>();
            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, DbContextOptions options)
        {
            services.AddPersistence().AddSingleton<AppDbContext>(new AppDbContext((DbContextOptions<AppDbContext>)options));

            return services;
        }
    }
}
