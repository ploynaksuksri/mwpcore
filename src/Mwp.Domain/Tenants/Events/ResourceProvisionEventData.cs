using System;

namespace Mwp.Tenants.Events
{
    public abstract class ResourceProvisionEventData
    {
        public Guid TenantId { get; set; }
        public Guid ResourceId { get; set; }
        public string SubscriptionId { get; set; }
        public string ResourceGroupName { get; set; }
        public string ServerName { get; set; }
    }
}