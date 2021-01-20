using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mwp.CloudService;
using Mwp.Tenants;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Saas.Tenants;

namespace Mwp.EntityFrameworkCore.Repositories.Tenants
{
    public class TenantExRepository : EfCoreRepository<MwpDbContext, TenantEx, Guid>, ITenantExRepository
    {
        public TenantExRepository(IDbContextProvider<MwpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<Tenant>> GetTenantListByLocationIdAsync(int locationId, string sorting = null, CancellationToken cancellationToken = default)
        {
            return await DbSet.Include(tx => tx.Tenant)
                .ThenInclude(tx => tx.ConnectionStrings)
                .Include(tx => tx.TenantResources)
                .ThenInclude(tx => tx.CloudServiceOption)
                .ThenInclude(tx => tx.CloudService)
                .ThenInclude(tx => tx.CloudServiceType)
                .Where(tx => tx.TenantResources.FirstOrDefault(r => r.CloudServiceOption.CloudService.CloudServiceTypeId == (int)CloudServiceTypes.Databases).CloudServiceLocationId == locationId)
                .Select(tx => tx.Tenant)
                .ToListAsync(cancellationToken);
        }

        public async Task<TenantEx> GetByTenantIdAsync(Guid tenantId, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var query = includeDetails ? WithDetails() : DbSet;

            var result = await query.FirstOrDefaultAsync(tx => tx.TenantId == tenantId, cancellationToken);

            if (result == null)
            {
                throw new EntityNotFoundException($"Cannot find TenantEx with Tenant Id {tenantId}");
            }

            return result;
        }

        public async Task<List<TenantEx>> GetListAsync(
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            string filter = null,
            Guid? tenantParentId = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            var filterIsGiven = !filter.IsNullOrWhiteSpace();
            var query = includeDetails ? WithDetails() : DbSet;

            if (filterIsGiven && !includeDetails)
            {
                query = query.Include(tx => tx.Tenant);
            }

            return await query.WhereIf(filterIsGiven, tx => tx.Tenant.Name.Contains(filter))
                .WhereIf(tenantParentId.HasValue, tx => tx.TenantParentId == tenantParentId)
                .OrderBy(sorting ?? nameof(TenantEx.TenantId))
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filter = null,
            Guid? tenantParentId = null,
            CancellationToken cancellationToken = default)
        {
            var filterIsGiven = !filter.IsNullOrWhiteSpace();
            return await DbSet.IncludeIf(filterIsGiven, tx => tx.Tenant)
                .WhereIf(filterIsGiven, tx => tx.Tenant.Name.Contains(filter))
                .WhereIf(tenantParentId.HasValue, tx => tx.TenantParentId == tenantParentId)
                .CountAsync(cancellationToken);
        }

        public async Task<bool> IsSchemaExist(CancellationToken cancellationToken = default)
        {
            var connectionString = DbContext.Database.GetDbConnection()?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString) || connectionString == "Data Source=:memory:")
            {
                return false;
            }

            const string query = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @mwpSchemaName";
            var parameters = new Dictionary<string, object> { { "@mwpSchemaName", MwpConsts.DbSchema } };

            using (var command = DbContext.Database.CreateQueryCommand(query, parameters))
            {
                var result = await command.ExecuteScalarAsync(cancellationToken);
                return (int)result > 0;
            }
        }
    }
}