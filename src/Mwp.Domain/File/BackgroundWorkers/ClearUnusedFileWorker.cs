using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Mwp.File.BackgroundWorkers
{
    public class ClearUnusedFileWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private DateTime _lastRunTimestamp = DateTime.MinValue;

        public ClearUnusedFileWorker(
            AbpTimer timer,
            IServiceScopeFactory serviceScopeFactory
        ) : base(
            timer,
            serviceScopeFactory)
        {
            Timer.Period = 15 * 60 * 1000; //15 min.
        }

        protected override async Task DoWorkAsync(
            PeriodicBackgroundWorkerContext workerContext)
        {
            var now = DateTime.Now;
            if (now.Subtract(_lastRunTimestamp).TotalHours < 24)
            {
                return;
            }

            _lastRunTimestamp = now;
            Logger.LogInformation("Starting: Clear Unused files in all tenants");

            var fileService = workerContext
                .ServiceProvider
                .GetRequiredService<IFileStorageClient>();

            await fileService.ClearUnusedFileInAllTenants(now);

            Logger.LogInformation("Completed: Clear Unused files in all tenants");
        }
    }
}