using System;
using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;

namespace Mwp.EventBus.ServiceBus
{
    public class MwpServiceBusEventBusOptions : ISingletonDependency
    {
        public string ConnectionString { get; set; }

        public ConcurrentDictionary<Type, string> Pulishers { get; set; }

        public ConcurrentDictionary<Type, string> Consummers { get; set; }

        public MwpServiceBusEventBusOptions()
        {
            Pulishers = new ConcurrentDictionary<Type, string>();
            Consummers = new ConcurrentDictionary<Type, string>();
        }
    }
}