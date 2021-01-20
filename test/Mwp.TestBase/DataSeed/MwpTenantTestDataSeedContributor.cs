using System;
using System.Threading.Tasks;
using Mwp.CloudService;
using Mwp.Tenants;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Security.Encryption;
using Volo.Abp.Uow;
using Volo.Saas.Tenants;

namespace Mwp.DataSeed
{
    public class MwpTenantTestDataSeedContributor : MwpTestDataSeedBase, IDataSeedContributor
    {
        public MwpTenantTestDataSeedContributor(
            IGuidGenerator guidGenerator,
            ITenantRepository tenantRepository,
            ITenantExRepository tenantExRepository,
            IRepository<TenantResource> tenantResourceRepository,
            IStringEncryptionService encryptionService)
            : base(guidGenerator, tenantRepository, tenantExRepository, tenantResourceRepository, encryptionService)
        {
        }

        [UnitOfWork]
        public async Task SeedAsync(DataSeedContext context)
        {
            var tenantParentId = await CreateTenant1IfNotExist();
            await CreateTenant2IfNotExist(tenantParentId);
            await CreateTenant3IfNotExist();
        }

        protected async Task<Guid> CreateTenant1IfNotExist()
        {
            const string tenantName = "T1";

            var existingTenant = await FindTenant(tenantName);
            if (existingTenant != null)
            {
                return existingTenant.Id;
            }

            var (tenantId, _) = await CreateMwpTenant(tenantName, Guid.Empty);
            return tenantId;
        }

        protected async Task CreateTenant2IfNotExist(Guid tenantParentId)
        {
            const string tenantName = "T2";

            if (!await CheckIfTenantExists(tenantName))
            {
                var (tenantId, tenantExId) = await CreateMwpTenant(tenantName, tenantParentId);
                await InsertTenantResource(tenantId, tenantExId, CloudServiceLocations.AustraliaEast, CloudServiceOptions.DatabaseStandard, $"ConnectionStringOf{tenantName}");
                await InsertTenantResource(tenantId, tenantExId, CloudServiceLocations.AustraliaEast, CloudServiceOptions.StorageStandard, $"StorageConnectionStringOf{tenantName}");
            }
        }

        protected async Task CreateTenant3IfNotExist()
        {
            const string tenantName = "T3";

            if (!await CheckIfTenantExists(tenantName))
            {
                await CreateMwpTenant(tenantName, Guid.Empty, false);
            }
        }
    }
}