using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mwp.ServiceBus;
using Mwp.ServiceBus.Consumer;
using Mwp.ServiceBus.Publisher;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace Mwp.EventBus.ServiceBus
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IDistributedEventBus), typeof(ServiceBusDistributedEventBus))]
    public class ServiceBusDistributedEventBus : EventBusBase, IDistributedEventBus, ISingletonDependency
    {
        protected MwpServiceBusEventBusOptions MwpServiceBusEventBusOptions { get; }
        protected AbpDistributedEventBusOptions AbpDistributedEventBusOptions { get; }

        protected IServiceBusMessagePublisherFactory MessagePublisherFactory { get; }
        protected ConcurrentDictionary<Type, IServiceBusMessagePublisher> PulisherFactories { get; }

        protected IServiceBusMessageConsumerFactory MessageConsumerFactory { get; }
        protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }
        protected ConcurrentDictionary<string, Type> SubscribedEventTypes { get; }

        private readonly ServiceBusSerializer _serializer;

        public ServiceBusDistributedEventBus(
            IOptions<MwpServiceBusEventBusOptions> options,
            IOptions<AbpDistributedEventBusOptions> distributedEventBusOptions,
            IServiceBusMessageConsumerFactory messageConsumerFactory,
            IServiceBusMessagePublisherFactory messagePublisherFactory,
            ServiceBusSerializer serializer,
            IServiceScopeFactory serviceScopeFactory,
            ICurrentTenant currentTenant)
            : base(serviceScopeFactory, currentTenant)
        {
            MessageConsumerFactory = messageConsumerFactory;
            MessagePublisherFactory = messagePublisherFactory;
            AbpDistributedEventBusOptions = distributedEventBusOptions.Value;
            MwpServiceBusEventBusOptions = options.Value;

            HandlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
            PulisherFactories = new ConcurrentDictionary<Type, IServiceBusMessagePublisher>();

            SubscribedEventTypes = new ConcurrentDictionary<string, Type>();

            _serializer = serializer;
        }

        public void Initialize()
        {
            MwpServiceBusEventBusOptions.Pulishers.Locking(publishers =>
            {
                foreach (var publisher in publishers)
                {
                    InitializePublisher(publisher.Key, publisher.Value);
                }
            });

            MwpServiceBusEventBusOptions.Consummers.Locking(consummers =>
            {
                foreach (var consummer in consummers)
                {
                    InitializeConsumer(consummer.Key, consummer.Value);
                }
            });

            SubscribeHandlers(AbpDistributedEventBusOptions.Handlers);
        }

        public override async Task PublishAsync(Type eventType, object eventData)
        {
            if (!PulisherFactories.ContainsKey(eventType))
            {
                return;
            }

            using var publisher = PulisherFactories[eventType];
            var body = _serializer.Serialize(eventData);
            await publisher.SendMessage(body);
        }


        public IDisposable Subscribe<TEvent>(IDistributedEventHandler<TEvent> handler) where TEvent : class
        {
            return Subscribe(typeof(TEvent), handler);
        }

        public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            var handlerList = GetOrCreateHandlerFactories(eventType);

            if (factory.IsInFactories(handlerList))
            {
                return NullDisposable.Instance;
            }

            handlerList.Add(factory);

            return new EventHandlerFactoryUnregistrar(this, eventType, factory);
        }

        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEvent))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as SingleInstanceHandlerFactory;

                            if (!(singleInstanceFactory?.HandlerInstance is ActionEventHandler<TEvent> actionHandler))
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        public override void Unsubscribe(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                            factory is SingleInstanceHandlerFactory handlerFactory &&
                            handlerFactory.HandlerInstance == handler
                    );
                });
        }

        public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        public override void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            var focusHandlers = HandlerFactories
                .Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key));

            foreach (var handlerFactory in focusHandlers)
            {
                handlerFactoryList.Add(new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }


        #region Private Methods

        private void InitializePublisher(Type eventType, string queueName)
        {
            var publisher = MessagePublisherFactory.Create(CreateQueueConfiguration(queueName));
            PulisherFactories[eventType] = publisher;
        }

        private void InitializeConsumer(Type eventType, string queueName)
        {
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            var consumer = MessageConsumerFactory.Create(CreateQueueConfiguration(queueName));
            consumer.OnMessageReceived((message, cancellationToken) => ProcessEventAsync(eventName, message, cancellationToken));
        }

        private QueueClientDeclareConfiguration CreateQueueConfiguration(string queueName)
        {
            return new QueueClientDeclareConfiguration(MwpServiceBusEventBusOptions.ConnectionString, queueName, 1, true);
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return HandlerFactories.GetOrAdd(
                eventType,
                type =>
                {
                    var eventName = EventNameAttribute.GetNameOrDefault(type);
                    SubscribedEventTypes[eventName] = type;
                    return new List<IEventHandlerFactory>();
                }
            );
        }

        private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
        {
            return handlerEventType == targetEventType || handlerEventType.IsAssignableFrom(targetEventType);
        }

        private async Task ProcessEventAsync(string eventName, Message message, CancellationToken token)
        {
            var eventType = SubscribedEventTypes.GetOrDefault(eventName);
            if (eventType == null)
            {
                return;
            }

            var eventData = _serializer.Deserialize(message.Body, eventType);

            await TriggerHandlersAsync(eventType, eventData);
        }

        #endregion
    }
}