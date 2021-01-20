using System;

namespace Mwp.ServiceBus.Publisher
{
    public interface IServiceBusMessagePublisherFactory : IDisposable
    {
        IServiceBusMessagePublisher Create(QueueClientDeclareConfiguration queueConfiguration);
    }
}