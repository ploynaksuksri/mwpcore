using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Entities
{
    public class EntityEntity : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid MemberOfId { get; set; }

        public virtual Guid MemberId { get; set; }

        public virtual Entity MemberOf { get; set; }

        public virtual Entity Member { get; set; }

        public EntityEntity(Guid memberOfId, Guid memberId)
        {
            MemberOfId = memberOfId;
            MemberId = memberId;
        }
    }
}