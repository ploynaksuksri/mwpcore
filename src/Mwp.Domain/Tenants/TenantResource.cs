using System;
using System.ComponentModel.DataAnnotations.Schema;
using Mwp.CloudService;
using Mwp.Provision;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Tenants
{
    public class TenantResource : FullAuditedEntity<Guid>
    {
        public int CloudServiceOptionId { get; set; }
        public CloudServiceOption CloudServiceOption { get; set; }
        public int CloudServiceLocationId { get; set; }
        public CloudServiceLocation CloudServiceLocation { get; set; }

        public Guid TenantId { get; protected set; }
        public Guid TenantExId { get; protected set; }
        public virtual TenantEx TenantEx { get; set; }

        public string Name { get; set; }
        public string SubscriptionId { get; set; }
        public string ResourceGroup { get; set; }
        public bool? IsActive { get; set; }
        public ProvisionStatus ProvisionStatus { get; set; }
        public string ConnectionString { get; set; }
        public string ServerName { get; set; }

        #region NotMapped

        [NotMapped]
        public bool IsProvisionRequired { get; set; }

        [NotMapped]
        public string ElasticPoolName { get; set; }

        [NotMapped]
        public string AdminEmailAddress { get; set; }

        [NotMapped]
        public string AdminPassword { get; set; }

        #endregion

        protected TenantResource()
        {

        }

        public TenantResource(Guid tenantId, Guid tenantExId)
        {
            TenantId = tenantId;
            TenantExId = tenantExId;
        }
    }
}