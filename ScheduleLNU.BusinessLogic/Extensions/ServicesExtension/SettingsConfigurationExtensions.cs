using Microsoft.Extensions.DependencyInjection;
using ScheduleLNU.BusinessLogic.Services;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.BusinessLogic.Extensions.ServicesExtension
{
    public static class SettingsConfigurationExtensions
    {
        public static IServiceCollection AddSettingServices(this IServiceCollection services)
        {
            return services.AddScoped<IThemeService, ThemeService>()
                           .AddScoped<IEventStyleService, EventStyleService>()
                           .AddScoped<IAuthService, AuthService>();
        }
    }
}
