using System;
using System.IO;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Mwp.Secrets;
using Serilog;
using Serilog.Events;

namespace Mwp.Configuration
{
    public static class ConfigurationUtils
    {
        public static IConfiguration BuildConfiguration(IHostEnvironment env = null, IConfigurationBuilder config = null)
        {
            config ??= new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            if (env != null && !env.IsDevelopment())
            {
                var settings = config.Build();

                var keyVaultUri = AzureKeyVaultSecretClient.GetKeyVaultUri(settings["KeyVaultName"]);
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                config.AddAzureKeyVault(keyVaultUri, keyVaultClient, new DefaultKeyVaultSecretManager());
            }

            return config.Build();
        }

        public static void ConfigureLogging(IConfiguration appSettings = null)
        {
            var logSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("serilog.json", false)
                .Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(logSettings);

            if (appSettings != null)
            {
                var instrumentationKey = appSettings[MwpConsts.ApplicationInsightsInstrumentationKey];
                var minLevel = appSettings[MwpConsts.ApplicationInsightsMinLevel];

#if DEBUG
                loggerConfiguration = loggerConfiguration
                    .MinimumLevel.Debug()
                    .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "Logs/logs.txt"));
#endif

                if (!string.IsNullOrEmpty(instrumentationKey) && Enum.TryParse(minLevel ?? "Warning", out LogEventLevel logEventLevel))
                {
                    loggerConfiguration = loggerConfiguration
                        .WriteTo.ApplicationInsights(instrumentationKey,
                            TelemetryConverter.Traces, logEventLevel);
                }
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}