using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mwp.Configuration;

namespace Mwp.DbMigrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConfigurationUtils.ConfigureLogging();

            await CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) => logging.ClearProviders())
                .ConfigureAppConfiguration((context, config) =>
                {
                    var setting = ConfigurationUtils.BuildConfiguration(context.HostingEnvironment, config);
                    ConfigurationUtils.ConfigureLogging(setting);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var (locationId, rollback) = ExtractArguments(args);

                    services.AddHostedService(serviceProvider => new DbMigratorHostedService(
                        serviceProvider.GetService<IHostApplicationLifetime>(),
                        services.GetConfiguration(),
                        locationId,
                        rollback
                    ));
                });

        private static (int?, bool) ExtractArguments(string[] args)
        {
            int? locationId = null;
            var rollback = false;

            foreach (var arg in args)
            {
                if (arg == "-rollback")
                {
                    rollback = true;
                }

                if (int.TryParse(arg, out var location))
                {
                    locationId = location;
                }
            }

            return (locationId, rollback);
        }
    }
}