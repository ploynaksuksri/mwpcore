using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Xero.Repositories
{
    public interface IXeroConnectionRepository : IRepository<XeroConnection, Guid>
    {
        Task<List<XeroConnection>> GetByMwpUserId(Guid mwpUserId, CancellationToken cancellationToken = default);

        Task<XeroConnection> GetAsync(
            Guid mwpUserId,
            Guid xeroTenantId,
            CancellationToken cancellationToken = default);

        Task<XeroConnection> GetAsync(
            Guid xeroConnectionId,
            Guid xeroTenantId,
            Guid mwpUserId,
            Guid authenticationEventId,
            CancellationToken cancellationToken = default);
    }
}