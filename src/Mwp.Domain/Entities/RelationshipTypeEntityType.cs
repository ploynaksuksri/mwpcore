using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Entities
{
    public class RelationshipTypeEntityType : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid RelationshipTypeId { get; set; }

        public virtual Guid EntityTypeId { get; set; }

        public virtual RelationshipType RelationshipType { get; set; }

        public virtual EntityType EntityType { get; set; }

        public RelationshipTypeEntityType(Guid relationshipTypeId, Guid entityTypeId)
        {
            RelationshipTypeId = relationshipTypeId;
            EntityTypeId = entityTypeId;
        }
    }
}