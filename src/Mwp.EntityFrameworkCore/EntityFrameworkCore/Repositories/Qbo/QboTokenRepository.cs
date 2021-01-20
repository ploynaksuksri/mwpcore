using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mwp.Qbo;
using Mwp.Qbo.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Mwp.EntityFrameworkCore.Repositories.Qbo
{
    public class QboTokenRepository : EfCoreRepository<MwpDbContext, QboToken, Guid>,
        IQboTokenRepository
    {
        public QboTokenRepository(IDbContextProvider<MwpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<QboToken> GetCurrentToken(
            Guid? mwpUserId,
            string qboTenantId,
            CancellationToken cancellationToken = default)
        {
            return await FilterValidToken(DbSet)
                .FirstOrDefaultAsync(u => u.MwpUserId == mwpUserId && u.QboTenantId == qboTenantId, cancellationToken);
        }

        public async Task<List<QboToken>> GetTokensByMwpUserId(Guid mwpUserId, CancellationToken cancellationToken = default)
        {
            return await FilterValidToken(DbSet)
                .Where(e => e.MwpUserId == mwpUserId).ToListAsync(cancellationToken);
        }

        protected IQueryable<QboToken> FilterValidToken(DbSet<QboToken> dbSet)
        {
            return dbSet.Where(e => e.IsRevoked == false && e.IsRefreshed == false && e.IsDeleted == false);
        }
    }
}