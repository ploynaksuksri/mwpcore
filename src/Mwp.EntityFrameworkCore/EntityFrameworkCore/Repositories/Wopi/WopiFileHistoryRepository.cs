using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mwp.Wopi;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Mwp.EntityFrameworkCore.Repositories.Wopi
{
    public class WopiFileHistoryRepository : EfCoreRepository<MwpDbContext, WopiFileHistory, Guid>, IWopiFileHistoryRepository
    {
        public WopiFileHistoryRepository(IDbContextProvider<MwpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<WopiFileHistory> GetLatestHistory(Guid fileId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(h => h.WopiFileId == fileId)
                .OrderByDescending(h => h.LastModificationTime)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}