using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Mwp.ServiceBus.Consumer
{
    public class ServiceBusMessageConsumerFactory : IServiceBusMessageConsumerFactory, ISingletonDependency
    {
        protected IServiceScope ServiceScope { get; }

        public ServiceBusMessageConsumerFactory(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScope = serviceScopeFactory.CreateScope();
        }

        public IServiceBusMessageConsumer Create(QueueClientDeclareConfiguration queueConfiguration)
        {
            var consumer = ServiceScope.ServiceProvider.GetRequiredService<ServiceBusMessageConsumer>();
            consumer.Initialize(queueConfiguration);
            return consumer;
        }

        public void Dispose()
        {
            ServiceScope?.Dispose();
        }
    }
}