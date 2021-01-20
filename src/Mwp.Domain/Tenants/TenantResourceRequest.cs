using System;

namespace Mwp.Tenants
{
    public class TenantResourceRequest
    {
        public Guid TenantId { get; set; }
        public Guid TenantExId { get; set; }
        public string TenantName { get; set; }
        public int LocationId { get; set; }
        public int DatabaseOptionId { get; set; }
        public int StorageOptionId { get; set; }
        public bool IsFreeTrial { get; set; }
        public string AdminEmailAddress { get; set; }
        public string AdminPassword { get; set; }
    }
}