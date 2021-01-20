using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mwp.Xero;
using Mwp.Xero.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Mwp.EntityFrameworkCore.Repositories.Xero
{
    public class XeroConnectionRepository : EfCoreRepository<MwpDbContext, XeroConnection, Guid>,
        IXeroConnectionRepository
    {
        public XeroConnectionRepository(IDbContextProvider<MwpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<XeroConnection>> GetByMwpUserId(Guid mwpUserId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(e => e.MwpUserId == mwpUserId && e.IsConnected)
                .ToListAsync(cancellationToken);
        }

        public async Task<XeroConnection> GetAsync(
            Guid mwpUserId,
            Guid xeroTenantId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.FirstOrDefaultAsync(e =>
                    e.MwpUserId == mwpUserId
                    && e.XeroTenantId == xeroTenantId
                    && e.IsConnected
                    && !e.IsDeleted
                , cancellationToken);
        }

        public async Task<XeroConnection> GetAsync(
            Guid xeroConnectionId,
            Guid xeroTenantId,
            Guid mwpUserId,
            Guid authenticationEventId,
            CancellationToken cancellationToken = default)
        {
            var connections = await DbSet.Where(e => e.XeroConnectionId == xeroConnectionId
                                                     && e.MwpUserId == mwpUserId
                                                     && e.AuthenticationEventId == authenticationEventId
                                                     && e.XeroTenantId == xeroTenantId
                                                     && e.IsDeleted == false).ToListAsync(cancellationToken);

            return connections.Any() ? connections.FirstOrDefault() : null;
        }
    }
}