using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Xero.Repositories
{
    public interface IXeroTenantRepository : IRepository<XeroTenant, Guid>
    {
        Task<List<XeroTenant>> GetByMwpTenantId(Guid? mwpTenantId, CancellationToken cancellationToken = default);
    }
}