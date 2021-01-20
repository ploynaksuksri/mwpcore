using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Threading;

namespace Mwp.ServiceBus.Consumer
{
    public class ServiceBusMessageConsumer : IServiceBusMessageConsumer, ITransientDependency
    {
        protected ConcurrentBag<Func<Message, CancellationToken, Task>> Callbacks { get; }
        protected AbpTimer Timer { get; }
        protected IExceptionNotifier ExceptionNotifier { get; }
        protected QueueClientDeclareConfiguration QueueConfiguration { get; private set; }

        protected static QueueClient QueueClient;
        protected ILogger<ServiceBusMessageConsumer> Logger { get; set; }

        public ServiceBusMessageConsumer(
            AbpTimer timer,
            IExceptionNotifier exceptionNotifier)
        {
            Timer = timer;
            ExceptionNotifier = exceptionNotifier;
            Logger = NullLogger<ServiceBusMessageConsumer>.Instance;

            Callbacks = new ConcurrentBag<Func<Message, CancellationToken, Task>>();

            Timer.Period = 5000; //5 sec.
            Timer.Elapsed += Timer_Elapsed;
            Timer.RunOnStart = true;
        }


        public void Initialize([NotNull] QueueClientDeclareConfiguration queue)
        {
            QueueConfiguration = Check.NotNull(queue, nameof(queue));

            Timer.Start();
        }

        public void OnMessageReceived(Func<Message, CancellationToken, Task> callback)
        {
            Callbacks.Add(callback);
        }

        protected virtual void Timer_Elapsed(object sender, EventArgs e)
        {
            EnsureQueueClientIsOpen();
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
                var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = QueueConfiguration.MaxConcurrentCalls,
                    AutoComplete = QueueConfiguration.AutoComplete
                };

                QueueClient = new QueueClient(QueueConfiguration.ConnectionString, QueueConfiguration.QueueName);
                QueueClient.RegisterMessageHandler(HandleIncomingMessage, messageHandlerOptions);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogLevel.Warning);
                AsyncHelper.RunSync(() => ExceptionNotifier.NotifyAsync(ex, LogLevel.Warning));
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Logger.LogError("Message handler encountered an exception", exceptionReceivedEventArgs.Exception);
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Logger.LogError($"- Endpoint: {context.Endpoint}");
            Logger.LogError($"- Entity Path: {context.EntityPath}");
            Logger.LogError($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        protected virtual async Task HandleIncomingMessage(Message message, CancellationToken token)
        {
            try
            {
                foreach (var callback in Callbacks)
                {
                    await callback(message, token);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                await ExceptionNotifier.NotifyAsync(ex);
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
            Timer.Stop();
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