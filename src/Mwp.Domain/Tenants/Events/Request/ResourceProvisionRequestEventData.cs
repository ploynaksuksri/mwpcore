using System;
using Newtonsoft.Json;

namespace Mwp.Tenants.Events.Request
{
    public abstract class ResourceProvisionRequestEventData : ResourceProvisionEventData
    {
        public int LocationId { get; set; }
        public int CloudServiceOptionId { get; set; }
    }

    [Serializable]
    public class DatabaseProvisionRequestEventData : ResourceProvisionRequestEventData
    {
        public string ElasticPoolName { get; set; }

        [JsonProperty("name")]
        public string DatabaseName { get; set; }
    }

    [Serializable]
    public class StorageProvisionRequestEventData : ResourceProvisionRequestEventData
    {
        [JsonProperty("name")]
        public string StorageName { get; set; }
    }
}