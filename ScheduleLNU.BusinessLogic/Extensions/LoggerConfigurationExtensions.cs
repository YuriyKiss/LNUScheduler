using Microsoft.Extensions.Configuration;
using Serilog;

namespace ScheduleLNU.BusinessLogic.Extensions
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration ConfigureFromJSON(this LoggerConfiguration configuration)
        {
            return configuration.ReadFrom
                .Configuration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build());
        }
    }
}
