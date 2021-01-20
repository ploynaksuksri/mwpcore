using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Communications
{
    public class CommunicationEmail : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid CommunicationId { get; set; }

        public virtual Guid EmailId { get; set; }

        public virtual Communication Communication { get; set; }

        public virtual Email Email { get; set; }

        public CommunicationEmail(Guid communicationId, Guid emailId)
        {
            CommunicationId = communicationId;
            EmailId = emailId;
        }
    }
}