using System;
using System.Collections.Generic;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Mwp.Entities;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Engagements
{
    public class Engagement : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual Guid EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public virtual ICollection<Workbook> Workbooks { get; set; }

        public Engagement()
        {

        }

        public Engagement(Guid id, Guid? tenantId, string name, Guid entityId, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), EngagementConsts.NameMaxLength, EngagementConsts.NameMinLength);
            Name = name;

            EntityId = entityId;
            IsActive = isActive;
        }
    }
}