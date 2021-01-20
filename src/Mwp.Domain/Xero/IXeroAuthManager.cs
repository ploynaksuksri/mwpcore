using System;
using System.Threading.Tasks;

namespace Mwp.Xero
{
    public interface IXeroAuthManager
    {
        Task<XeroToken> GetCurrentXeroToken(Guid? mwpUserId, Guid xeroTenantId);

        Task<XeroToken> RefreshAccessToken(Guid mwpUserId, Guid xeroTenantId);

        Task RemoveConnection(Guid mwpUserId, Guid xeroTenantId);

        Task<XeroToken> GetAccessToken(string code, string state);

        Task<string> GetAuthoriseUrl(Guid mwpUserId, Guid mwpTenantId);
    }
}