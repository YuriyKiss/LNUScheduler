using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScheduleLNU.DataAccess;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;

namespace ScheduleLNU.BusinessLogic.Extensions.ServicesExtension
{
    public static class DatabaseConfigurationExtensions
    {
        public static IServiceCollection AddDbConfiguration(this IServiceCollection services, string connectionString)
        {
            return services.AddSchedulesDb(connectionString)
                           .AddAspNetIdentityDbContext();
        }

        public static IServiceCollection AddSchedulesDb(this IServiceCollection services, string connectionString)
        {
            return services.AddScoped<IdentityDbContext<Student>, DataContext>()
                           .AddDbContext<DataContext>(opt => opt.UseNpgsql(connectionString))
                           .AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        public static IServiceCollection AddAspNetIdentityDbContext(this IServiceCollection services)
        {
            services.AddIdentity<Student, IdentityRole>(options =>
                           {
                               options.SignIn.RequireConfirmedAccount = true;
                               options.SignIn.RequireConfirmedEmail = true;
                               options.Password.RequiredLength = 6;
                               options.Password.RequireNonAlphanumeric = false;
                               options.Password.RequireUppercase = false;
                               options.Password.RequireLowercase = false;
                           })
                    .AddEntityFrameworkStores<DataContext>()
                    .AddDefaultTokenProviders();
            return services;
        }
    }
}
