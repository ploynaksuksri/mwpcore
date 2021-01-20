using System;
using System.Threading.Tasks;

namespace Mwp.Qbo
{
    public interface IQboOAuth
    {
        string GetAuthoriseUrl(Guid mwpUserId, Guid mwpTenantId);

        Task<Tuple<QboToken, QboTenant>> GetAccessTokenAsync(string code, string state, string realmId);

        Task<QboToken> RefreshAccessToken(QboToken currentToken);

        Task RemoveConnection(QboToken token);
    }
}