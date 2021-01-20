using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Entities
{
    public class EntityGroupEntity : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid EntityGroupId { get; set; }

        public virtual Guid EntityId { get; set; }

        public virtual EntityGroup EntityGroup { get; set; }

        public virtual Entity Entity { get; set; }

        public EntityGroupEntity(Guid entityGroupId, Guid entityId)
        {
            EntityGroupId = entityGroupId;
            EntityId = entityId;
        }
    }
}