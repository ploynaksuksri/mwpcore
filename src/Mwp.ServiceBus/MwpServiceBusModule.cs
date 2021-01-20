using Volo.Abp.Json;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Mwp.ServiceBus
{
    [DependsOn(
        typeof(AbpJsonModule),
        typeof(AbpThreadingModule)
    )]
    public class MwpServiceBusModule : AbpModule
    {
    }
}