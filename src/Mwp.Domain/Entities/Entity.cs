using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Mwp.Engagements;
using Mwp.Financials;
using Mwp.Tenants;
using Volo.Abp;

namespace Mwp.Entities
{
    public class Entity : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual Guid EntityTypeId { get; set; }

        public virtual EntityType EntityType { get; set; }

        public virtual Guid TenantExId { get; set; }

        public virtual TenantEx TenantEx { get; set; }

        public virtual Guid? EntityGroupId { get; set; }

        public virtual ICollection<EntityGroupEntity> EntityGroupEntity { get; set; }

        public virtual ICollection<Engagement> Engagements { get; set; }

        public virtual ICollection<EntityCommunication> EntityCommunication { get; set; }

        public virtual ICollection<EntityEntity> MembersOf { get; set; }

        public virtual ICollection<EntityEntity> Members { get; set; }

        // 1 to 0..1 relationship
        public virtual ICollection<Ledger> Ledger { get; set; }

        public Entity()
        {

        }

        public Entity(Guid id, Guid? tenantId, string name, Guid entityTypeId, Guid tenantExId, Guid? entityGroupId, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), EntityConsts.NameMaxLength, EntityConsts.NameMinLength);
            Name = name;

            EntityTypeId = entityTypeId;
            TenantExId = tenantExId;
            EntityGroupId = entityGroupId;
            IsActive = isActive;
        }
    }
}