using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Communications
{
    public class CommunicationWebsite : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid CommunicationId { get; set; }

        public virtual Guid WebsiteId { get; set; }

        public virtual Communication Communication { get; set; }

        public virtual Website Website { get; set; }

        public CommunicationWebsite(Guid communicationId, Guid websiteId)
        {
            CommunicationId = communicationId;
            WebsiteId = websiteId;
        }
    }
}