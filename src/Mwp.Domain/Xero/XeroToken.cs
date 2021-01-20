using System;
using Mwp.ThirdPartyOAuth;

namespace Mwp.Xero
{
    public class XeroToken : OAuthToken
    {
        public Guid AuthenticationEventId { get; set; }
    }
}