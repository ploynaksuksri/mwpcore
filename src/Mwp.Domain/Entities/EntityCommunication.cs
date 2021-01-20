using System;
using Mwp.Communications;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Entities
{
    public class EntityCommunication : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid EntityId { get; set; }

        public virtual Guid CommunicationId { get; set; }

        public virtual Entity Entity { get; set; }

        public virtual Communication Communication { get; set; }

        public EntityCommunication(Guid entityId, Guid communicationId)
        {
            EntityId = entityId;
            CommunicationId = communicationId;
        }
    }
}