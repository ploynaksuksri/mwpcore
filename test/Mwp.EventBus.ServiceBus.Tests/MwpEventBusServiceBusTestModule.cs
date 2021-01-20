using Mwp.EventBus.ServiceBus;
using Volo.Abp.Modularity;

namespace Mwp.ServiceBus
{
    [DependsOn(
        typeof(MwpDomainModule),
        typeof(MwpEventBusServiceBusModule)
    )]
    public class MwpEventBusServiceBusTestModule : AbpModule
    {
    }
}