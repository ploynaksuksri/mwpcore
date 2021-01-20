using System;
using System.ComponentModel.DataAnnotations.Schema;
using Mwp.ThirdPartyOAuth;

namespace Mwp.Xero
{
    public class XeroTenant : ThirdPartyTenant
    {
        public Guid XeroTenantId { get; set; }
        public XeroTenantTypes TenantType { get; set; }

        [NotMapped]
        public XeroConnection XeroConnection { get; set; }
    }
}