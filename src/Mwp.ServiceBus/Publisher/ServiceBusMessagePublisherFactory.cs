using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace Mwp.ServiceBus.Publisher
{
    public class ServiceBusMessagePublisherFactory : IServiceBusMessagePublisherFactory, ISingletonDependency
    {
        protected IServiceScope ServiceScope { get; }

        public ServiceBusMessagePublisherFactory(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScope = serviceScopeFactory.CreateScope();
        }

        public IServiceBusMessagePublisher Create(QueueClientDeclareConfiguration queueConfiguration)
        {
            var publisher = ServiceScope.ServiceProvider.GetRequiredService<ServiceBusMessagePublisher>();
            publisher.Initialize(queueConfiguration);
            return publisher;
        }

        public void Dispose()
        {
            ServiceScope?.Dispose();
        }
    }
}