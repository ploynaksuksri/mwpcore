using Mwp.AzureStorage;
using Mwp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Mwp.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(MwpEntityFrameworkCoreDbMigrationsModule),
        typeof(MwpApplicationContractsModule),
        typeof(MwpAzureStorageModule)
    )]
    public class MwpDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = false;
            });
        }
    }
}