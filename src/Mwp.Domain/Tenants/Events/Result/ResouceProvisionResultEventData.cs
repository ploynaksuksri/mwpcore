using System;
using Mwp.Provision;

namespace Mwp.Tenants.Events.Result
{
    public abstract class ResouceProvisionResultEventData : ResourceProvisionEventData
    {
        public ProvisionStatus StatusId { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string AdminEmailAddress { get; set; }
        public string AdminPassword { get; set; }
    }

    [Serializable]
    public class DatabaseProvisionResultEventData : ResouceProvisionResultEventData
    {
    }

    [Serializable]
    public class StorageProvisionResultEventData : ResouceProvisionResultEventData
    {
    }
}