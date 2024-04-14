using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ScheduleLNU.BusinessLogic.Extensions;
using Serilog;

namespace ScheduleLNU.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ConfigureFromJSON()
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
