using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Communications
{
    public class CommunicationAddress : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid CommunicationId { get; set; }

        public virtual Guid AddressId { get; set; }

        public virtual Communication Communication { get; set; }

        public virtual Address Address { get; set; }

        public CommunicationAddress(Guid communicationId, Guid addressId)
        {
            CommunicationId = communicationId;
            AddressId = addressId;
        }
    }
}