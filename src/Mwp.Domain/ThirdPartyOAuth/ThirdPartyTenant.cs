using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.ThirdPartyOAuth
{
    public class ThirdPartyTenant : FullAuditedEntity<Guid>
    {
        public virtual string Name { get; set; }
        public virtual Guid? MwpTenantId { get; set; }
    }
}