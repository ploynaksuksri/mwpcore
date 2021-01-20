using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Xero.Repositories
{
    public interface IXeroTokenRepository : IRepository<XeroToken, Guid>
    {
        Task<XeroToken> GetCurrentToken(
            Guid? mwpUserId,
            Guid authenticationEventId,
            CancellationToken cancellationToken = default);
    }
}