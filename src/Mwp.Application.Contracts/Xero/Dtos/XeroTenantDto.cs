using System;

namespace Mwp.Xero.Dtos
{
    public class XeroTenantDto
    {
        public string Id { get; set; }
        public Guid XeroTenantId { get; set; }

        public Guid AuthenticationEventId { get; set; }

        public Guid TenantId { get; set; }

        public string TenantType { get; set; }
        public string Name { get; set; }

        public string CreatedDateUtc { get; set; }

        public string UpdatedDateUtc { get; set; }

        public bool IsConnected { get; set; }
    }
}