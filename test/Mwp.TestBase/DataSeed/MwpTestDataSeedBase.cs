using System;
using System.Threading.Tasks;
using Mwp.CloudService;
using Mwp.Provision;
using Mwp.Tenants;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Security.Encryption;
using Volo.Saas.Tenants;

namespace Mwp.DataSeed
{
    public class MwpTestDataSeedBase : ITransientDependency
    {
        protected IGuidGenerator _guidGenerator;
        protected ITenantRepository _tenantRepository;
		protected ITenantExRepository _tenantExRepository;
        protected IRepository<TenantResource> _tenantResourceRepository;
        protected IStringEncryptionService _encryptionService;

        public MwpTestDataSeedBase(
            IGuidGenerator guidGenerator,
            ITenantRepository tenantRepository,
            ITenantExRepository tenantExRepository,
            IRepository<TenantResource> tenantResourceRepository,
            IStringEncryptionService encryptionService)
        {
            _guidGenerator = guidGenerator;
            _tenantRepository = tenantRepository;
            _tenantExRepository = tenantExRepository;
            _tenantResourceRepository = tenantResourceRepository;
            _encryptionService = encryptionService;
        }

        protected Guid CreateGuid()
        {
            return _guidGenerator.Create();
        }

        protected async Task<Tenant> FindTenant(Guid id)
        {
            return await _tenantRepository.FindAsync(id);
        }

        protected async Task<Tenant> FindTenant(string name)
        {
            return await _tenantRepository.FindByNameAsync(name);
        }

        protected async Task<bool> CheckIfTenantExists(Guid id)
        {
            var tenant = await _tenantRepository.FindAsync(id);
            return (tenant != null);
        }

        protected async Task<bool> CheckIfTenantExists(string name)
        {
            var tenant = await _tenantRepository.FindByNameAsync(name);
            return (tenant != null);
        }

        protected async Task<Tenant> CreateTenant(string tenantName)
        {
            var tenantManager = new TenantManager(_tenantRepository);
            var tenant = await tenantManager.CreateAsync(tenantName);
            await _tenantRepository.InsertAsync(tenant, true);
            return tenant;
        }

        protected async Task UpdateTenantDefaultConnectionString(Tenant tenant)
        {
            tenant.SetDefaultConnectionString(_encryptionService.Encrypt($"ConnectionStringOf{tenant.Name}"));
            await _tenantRepository.UpdateAsync(tenant, true);
        }

        protected async Task<TenantEx> CreateTenantEx(Guid tenantId, Guid tenantParentId)
        {
            var tenantEx = new TenantEx(tenantId, tenantParentId);
            await _tenantExRepository.InsertAsync(tenantEx, true);
            return tenantEx;
        }

        protected async Task<TenantResource> InsertTenantResource(Guid tenantId, Guid tenantExId, CloudServiceLocations cloudServiceLocation, CloudServiceOptions cloudServiceOption, string connectionString)
        {
            return await _tenantResourceRepository.InsertAsync(new TenantResource(tenantId, tenantExId)
            {
                CloudServiceLocationId = (int)cloudServiceLocation,
                CloudServiceOptionId = (int)cloudServiceOption,
                ConnectionString = _encryptionService.Encrypt(connectionString),
                IsActive = true,
                ProvisionStatus = ProvisionStatus.Success
            }, true);
        }

        protected async Task<(Guid tenantId, Guid tenantExId)> CreateMwpTenant(string tenantName, Guid tenantParentId, bool setDefaultConnectionString = true)
        {
            var tenant = await CreateTenant(tenantName);
            var tenantId = tenant.Id;
            if (setDefaultConnectionString)
            {
                await UpdateTenantDefaultConnectionString(tenant);
            }
            var tenantEx = await CreateTenantEx(tenantId, tenantParentId);
            var tenantExId = tenantEx.Id;
            return (tenantId, tenantExId);
        }
    }
}