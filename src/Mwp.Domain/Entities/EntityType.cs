using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Entities
{
    public class EntityType : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual ICollection<Entity> Entities { get; set; }

        public virtual ICollection<RelationshipTypeEntityType> RelationshipTypeEntityType { get; set; }

        public EntityType()
        {

        }

        public EntityType(Guid id, Guid? tenantId, string name, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), EntityTypeConsts.NameMaxLength, EntityTypeConsts.NameMinLength);
            Name = name;

            IsActive = isActive;
        }
    }
}