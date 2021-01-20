using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Tenants
{
    public class TenantType : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual ICollection<TenantEx> TenantExes { get; set; }

        public TenantType()
        {

        }

        public TenantType(Guid id, Guid? tenantId, string name, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), TenantTypeConsts.NameMaxLength, TenantTypeConsts.NameMinLength);
            Name = name;

            IsActive = isActive;
        }
    }
}