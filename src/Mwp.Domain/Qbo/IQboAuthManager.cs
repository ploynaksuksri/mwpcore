using System;
using System.Threading.Tasks;

namespace Mwp.Qbo
{
    public interface IQboAuthManager
    {
        Task<QboToken> GetCurrentToken(Guid? mwpUserId, string qboTenantId);

        Task<QboToken> RefreshAccessToken(Guid mwpUserId, string qboTenantId);

        Task RemoveConnection(Guid mwpUserId, string qboTenantId);

        Task<QboToken> GetAccessToken(string code, string state, string realmId);

        Task<string> GetAuthoriseUrl(Guid mwpUserId, Guid mwpTenantId);
    }
}