using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Mwp.Configuration;
using Serilog;

namespace Mwp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ConfigurationUtils.ConfigureLogging();

            try
            {
                Log.Information("Starting Mwp.HttpApi.Host.");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, config) =>
                    {
                        var setting = ConfigurationUtils.BuildConfiguration(context.HostingEnvironment, config);
                        ConfigurationUtils.ConfigureLogging(setting);
                    });
                    webBuilder.UseStartup<Startup>();
                })
                .UseAutofac()
                .UseSerilog();
    }
}