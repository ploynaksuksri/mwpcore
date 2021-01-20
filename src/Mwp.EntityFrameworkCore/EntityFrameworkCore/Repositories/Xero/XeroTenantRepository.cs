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
    public class XeroTenantRepository : EfCoreRepository<MwpDbContext, XeroTenant, Guid>, IXeroTenantRepository
    {
        public XeroTenantRepository(IDbContextProvider<MwpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<XeroTenant>> GetByMwpTenantId(
            Guid? mwpTenantId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(u => u.MwpTenantId == mwpTenantId).ToListAsync(cancellationToken);
        }
    }
}