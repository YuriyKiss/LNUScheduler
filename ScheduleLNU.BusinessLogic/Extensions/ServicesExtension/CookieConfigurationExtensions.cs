using System;
using Microsoft.Extensions.DependencyInjection;
using ScheduleLNU.BusinessLogic.Services;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.BusinessLogic.Extensions.ServicesExtension
{
    public static class CookieConfigurationExtensions
    {
        public static IServiceCollection AddCookies(this IServiceCollection services)
        {
            services.AddScoped<ICookieService, CookieService>()
                .AddScoped<ILoginService, LoginService>()
                .ConfigureApplicationCookie(config =>
                {
                    config.LoginPath = "/authentication/login";
                    config.AccessDeniedPath = "/authentication/login";
                    config.ExpireTimeSpan = new TimeSpan(2, 0, 0);
                });

            return services;
        }
    }
}
