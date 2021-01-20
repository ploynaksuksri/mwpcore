using System;

namespace Mwp.Xero.Dtos
{
    public class XeroTokenDto
    {
        public Guid MwpUserId { get; set; }
        public string AccessToken { get; set; }
    }
}