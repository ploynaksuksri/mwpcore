using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mwp.Xero
{
    public interface IXeroOAuth
    {
        string GetAuthoriseUrl(Guid mwpUserId, Guid mwpTenantId);

        Task<XeroToken> GetAccessTokenAsync(string code, string state);

        Task<List<XeroTenant>> GetConnectionsAsync(XeroToken xeroToken);

        Task<XeroToken> RefreshAccessToken(XeroToken currentXeroToken);

        Task RemoveConnection(XeroToken xeroToken, XeroTenant xeroTenant);
    }
}