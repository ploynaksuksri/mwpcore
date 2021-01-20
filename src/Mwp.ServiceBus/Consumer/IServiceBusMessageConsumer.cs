using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Mwp.ServiceBus.Consumer
{
    public interface IServiceBusMessageConsumer : IDisposable
    {
        void OnMessageReceived(Func<Message, CancellationToken, Task> callback);
    }
}