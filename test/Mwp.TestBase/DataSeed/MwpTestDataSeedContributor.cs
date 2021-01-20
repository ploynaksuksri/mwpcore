using System;
using System.Threading.Tasks;
using Mwp.Tenants;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Security.Encryption;
using Volo.Abp.Uow;
using Volo.Saas.Tenants;

namespace Mwp.DataSeed
{
    public class MwpTestDataSeedContributor : MwpTestDataSeedBase, IDataSeedContributor
    {
        public MwpTestDataSeedContributor(
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
            // only seed data ONCE within the host context (e.g. context.TenantId is undefined)
            if (context.TenantId != null)
            {
                return;
            }

            await CreateTenant4IfNotExist();

            // CRM test data should be seeded in the tenant "T4" to avoid conflicts.
            // All models which require a Tenant Id should be seeded with data here.
        }

        protected async Task<Guid> CreateTenant4IfNotExist()
        {
            // MwpTenantTestDataSeedContributor seeds data to 3 tenants T1, T2 and T3 for essential Mwp Tenant tests.
            // Therefore, to avoid conflicts, MwpTestDataSeedContributor seeds data to the fourth tenant which
            // must come after the other 3 tenants in an ordered tenant list (hence the name "T4").
            const string tenantName = "T4";

            var existingTenant = await FindTenant(tenantName);
            if (existingTenant != null)
            {
                return existingTenant.Id;
            }

            var (tenantId, _) = await CreateMwpTenant(tenantName, Guid.Empty);
            return tenantId;
        }
    }
}