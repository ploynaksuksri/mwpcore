using System;

namespace Mwp.ServiceBus.Consumer
{
    public interface IServiceBusMessageConsumerFactory : IDisposable
    {
        IServiceBusMessageConsumer Create(QueueClientDeclareConfiguration queueConfiguration);
    }
}