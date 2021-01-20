using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Xero
{
    public class XeroConnection : FullAuditedEntity<Guid>
    {
        public Guid XeroConnectionId { get; set; }
        public Guid XeroTenantId { get; set; }
        public XeroTenant XeroTenant { get; set; }
        public Guid? MwpUserId { get; set; }
        public Guid AuthenticationEventId { get; set; }
        public bool IsConnected { get; set; }
    }
}