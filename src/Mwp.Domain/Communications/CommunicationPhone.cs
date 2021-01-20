using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Communications
{
    public class CommunicationPhone : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid CommunicationId { get; set; }

        public virtual Guid PhoneId { get; set; }

        public virtual Communication Communication { get; set; }

        public virtual Phone Phone { get; set; }

        public CommunicationPhone(Guid communicationId, Guid phoneId)
        {
            CommunicationId = communicationId;
            PhoneId = phoneId;
        }
    }
}