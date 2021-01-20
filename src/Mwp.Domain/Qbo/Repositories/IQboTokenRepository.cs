using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Qbo.Repositories
{
    public interface IQboTokenRepository : IRepository<QboToken, Guid>
    {
        Task<QboToken> GetCurrentToken(
            Guid? mwpUserId,
            string qboTenantId,
            CancellationToken cancellationToken = default);

        Task<List<QboToken>> GetTokensByMwpUserId(
            Guid mwpUserId,
            CancellationToken cancellationToken = default);
    }
}