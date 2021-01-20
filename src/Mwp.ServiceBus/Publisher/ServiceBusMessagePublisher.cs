using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Threading;

namespace Mwp.ServiceBus.Publisher
{
    public class ServiceBusMessagePublisher : IServiceBusMessagePublisher, ITransientDependency
    {
        protected static QueueClient QueueClient;

        public ILogger<ServiceBusMessagePublisher> Logger { get; set; }
        protected IExceptionNotifier ExceptionNotifier { get; }

        protected QueueClientDeclareConfiguration QueueConfiguration { get; private set; }

        public ServiceBusMessagePublisher(IExceptionNotifier exceptionNotifier)
        {
            ExceptionNotifier = exceptionNotifier;
            Logger = NullLogger<ServiceBusMessagePublisher>.Instance;
        }


        public void Initialize([NotNull] QueueClientDeclareConfiguration queue)
        {
            QueueConfiguration = Check.NotNull(queue, nameof(queue));
        }

        public async Task SendMessage(byte[] message)
        {
            Logger.LogInformation($"Sending message to queue {QueueConfiguration.QueueName}");
            try
            {
                EnsureQueueClientIsOpen();

                var byteArrayToSend = new Message(message);
                await QueueClient.SendAsync(byteArrayToSend);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw;
            }
        }

        protected virtual void EnsureQueueClientIsOpen()
        {
            if (QueueClientIsReady())
            {
                return;
            }

            CloseQueueAsync(); //ensure that closing queue is totally been closed

            try
            {
                QueueClient = new QueueClient(QueueConfiguration.ConnectionString, QueueConfiguration.QueueName);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogLevel.Warning);
                AsyncHelper.RunSync(() => ExceptionNotifier.NotifyAsync(ex, LogLevel.Warning));
            }
        }

        protected virtual void CloseQueueAsync()
        {
            if (QueueClient == null)
            {
                return;
            }

            try
            {
                QueueClient.CloseAsync();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogLevel.Warning);
            }
        }

        public virtual void Dispose()
        {
            CloseQueueAsync();
        }

        private bool QueueClientIsReady()
        {
            return QueueClient != null &&
                   !string.IsNullOrWhiteSpace(QueueClient.ClientId) &&
                   !QueueClient.IsClosedOrClosing
                   && string.Equals(QueueClient.QueueName, QueueConfiguration.QueueName, StringComparison.OrdinalIgnoreCase);
        }
    }
}