using System;
using System.Threading.Tasks;

namespace Mwp.ServiceBus.Publisher
{
    public interface IServiceBusMessagePublisher : IDisposable
    {
        Task SendMessage(byte[] message);
    }
}