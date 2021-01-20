using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Saas.Tenants;

namespace Mwp.Tenants
{
    public interface ITenantExRepository : IRepository<TenantEx, Guid>
    {
        Task<List<Tenant>> GetTenantListByLocationIdAsync(
            int locationId,
            string sorting = null,
            CancellationToken cancellationToken = default);

        Task<TenantEx> GetByTenantIdAsync(
            Guid tenantId,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<List<TenantEx>> GetListAsync(
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            string filter = null,
            Guid? tenantParentId = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            string filter = null,
            Guid? tenantParentId = null,
            CancellationToken cancellationToken = default);

        Task<bool> IsSchemaExist(CancellationToken cancellationToken = default);
    }
}