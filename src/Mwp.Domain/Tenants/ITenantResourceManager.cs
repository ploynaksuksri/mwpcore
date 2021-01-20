using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Saas.Tenants;

namespace Mwp.Tenants
{
    public interface ITenantResourceManager : IDomainService
    {
        Task ProvideTenantResources(TenantResourceRequest request);

        Task<TenantResource> ProvideTenantDatabase(TenantResourceRequest request);

        Task<TenantResource> ProvideTenantStorage(TenantResourceRequest request);

        Task<Tenant> UpdateTenantConnectionString(Guid tenantId, string connectionString);

        Task InitialiseTenantDatabase(Tenant tenant, string adminEmailAddress, string adminPassword);

        Task SetTenantIsActive(Guid tenantId, bool isActive);
    }
}