using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mwp.Engagements;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Content
{
    public class Component : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual ICollection<WorkpaperComponent> WorkpaperComponent { get; set; }

        public Component()
        {

        }

        public Component(Guid id, Guid? tenantId, string name, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), ComponentConsts.NameMaxLength, ComponentConsts.NameMinLength);
            Name = name;

            IsActive = isActive;
        }
    }
}