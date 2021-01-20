using Microsoft.Extensions.DependencyInjection;
using Mwp.ServiceBus;
using Volo.Abp;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace Mwp.EventBus.ServiceBus
{
    [DependsOn(
        typeof(AbpEventBusModule),
        typeof(MwpServiceBusModule))]
    public class MwpEventBusServiceBusModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context
                .ServiceProvider
                .GetRequiredService<ServiceBusDistributedEventBus>()
                .Initialize();
        }
    }
}