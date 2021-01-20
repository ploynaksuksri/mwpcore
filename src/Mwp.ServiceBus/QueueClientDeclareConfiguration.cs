using JetBrains.Annotations;

namespace Mwp.ServiceBus
{
    public class QueueClientDeclareConfiguration
    {
        [NotNull]
        public string ConnectionString { get; }

        [NotNull]
        public string QueueName { get; }

        public int MaxConcurrentCalls { get; set; }
        public bool AutoComplete { get; set; }

        public QueueClientDeclareConfiguration(
            [NotNull] string connectionString,
            [NotNull] string queueName,
            int maxConcurrentCalls,
            bool autoComplete)
        {
            ConnectionString = connectionString;
            QueueName = queueName;
            MaxConcurrentCalls = maxConcurrentCalls;
            AutoComplete = autoComplete;
        }
    }
}