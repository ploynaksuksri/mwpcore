using System;
using System.Collections.Generic;
using Mwp.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Saas.Tenants;

namespace Mwp.Tenants
{
    public class TenantEx : FullAuditedAggregateRoot<Guid>
    {
        public Guid TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }

        public bool? IsActive { get; set; }

        public Guid? TenantParentId { get; set; }

        public bool IsClient => (TenantParentId != Guid.Empty);

        public virtual List<TenantResource> TenantResources { get; set; }

        public Guid? TenantTypeId { get; set; }

        public virtual TenantType TenantType { get; set; }

        public virtual ICollection<Entity> Entities { get; set; }

        protected TenantEx()
        {

        }

        public TenantEx(Guid tenantId, Guid? tenantParentId = null, bool? isActive = null, Guid? tenantTypeId = null)
        {
            TenantId = tenantId;
            IsActive = isActive;
            TenantParentId = tenantParentId;
            TenantTypeId = tenantTypeId;
        }
    }
}