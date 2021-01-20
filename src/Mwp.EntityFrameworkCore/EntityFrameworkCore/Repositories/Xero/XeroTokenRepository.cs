using System;
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
    public class XeroTokenRepository : EfCoreRepository<MwpDbContext, XeroToken, Guid>, IXeroTokenRepository
    {
        public XeroTokenRepository(IDbContextProvider<MwpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<XeroToken> GetCurrentToken(
            Guid? mwpUserId,
            Guid authenticationEventId,
            CancellationToken cancellationToken = default)
        {
            return await FilterInvalidToken(DbSet)
                .FirstOrDefaultAsync(u => u.MwpUserId == mwpUserId
                                          && u.AuthenticationEventId == authenticationEventId, cancellationToken);
        }

        protected IQueryable<XeroToken> FilterInvalidToken(DbSet<XeroToken> dbSet)
        {
            return dbSet.Where(e => e.IsRevoked == false && e.IsRefreshed == false && e.IsDeleted == false);
        }
    }
}