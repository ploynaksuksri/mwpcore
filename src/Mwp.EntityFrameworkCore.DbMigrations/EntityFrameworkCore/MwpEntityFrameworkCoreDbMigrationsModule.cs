using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Mwp.EntityFrameworkCore
{
    [DependsOn(
        typeof(MwpEntityFrameworkCoreModule)
    )]
    public class MwpEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MwpMigrationsDbContext>();
        }
    }
}