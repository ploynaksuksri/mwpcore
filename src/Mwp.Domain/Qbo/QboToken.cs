using System;
using Mwp.ThirdPartyOAuth;

namespace Mwp.Qbo
{
    public class QboToken : OAuthToken
    {
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
        public string QboTenantId { get; set; }

        public bool IsRefreshTokenExpired() => IsExpired(RefreshTokenExpiresAtUtc);
    }
}