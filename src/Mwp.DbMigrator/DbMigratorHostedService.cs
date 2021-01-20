using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mwp.Data;
using Serilog;
using Volo.Abp;

namespace Mwp.DbMigrator
{
    public class DbMigratorHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _config;
        private readonly int? _locationId;
        private readonly bool _rollback;

        public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration config, int? locationId, bool rollback)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _config = config;
            _locationId = locationId;
            _rollback = rollback;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var application = AbpApplicationFactory.Create<MwpDbMigratorModule>(options =>
            {
                options.UseAutofac();
                options.Services.AddLogging(c => c.AddSerilog());
                options.Services.ReplaceConfiguration(_config);
            }))
            {
                application.Initialize();
                await application
                    .ServiceProvider
                    .GetRequiredService<MwpDbMigrationService>()
                    .MigrateAsync(_locationId, _rollback);

                application.Shutdown();

                _hostApplicationLifetime.StopApplication();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}