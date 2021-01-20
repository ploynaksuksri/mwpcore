using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.CloudService;
using Mwp.Tenants;
using Shouldly;
using Volo.Abp.Security.Encryption;
using Volo.Saas.Tenants;
using Xunit;

namespace Mwp.EntityFrameworkCore.Tenant
{
    /// <summary>
    ///     This test depend on data seeding in the MwpTenantTestDataSeedContributor
    /// </summary>
    public class MwpTenantExRepositoryTests : MwpEntityFrameworkCoreTestBase
    {
        readonly ITenantRepository TenantRepository;
        readonly ITenantExRepository TenantExRepository;

        protected IStringEncryptionService EncryptionService;

        public MwpTenantExRepositoryTests()
        {
            TenantRepository = GetRequiredService<ITenantRepository>();
            TenantExRepository = GetRequiredService<ITenantExRepository>();
            EncryptionService = GetRequiredService<IStringEncryptionService>();
        }

        [Fact]
        public async Task GetAsync_NotIncludDetail_Should_Return_Tenant_WithoutDetails()
        {
            var tenantInDb = (await TenantRepository.GetListAsync(filter: "T2")).First();

            var result = await TenantExRepository.GetAsync(tx => tx.TenantId == tenantInDb.Id, false);
            result.ShouldNotBeNull();
            result.TenantId.ShouldBe(tenantInDb.Id);
            result.Tenant.ShouldBeNull();
            result.TenantResources.ShouldBeNull();
        }

        [Fact]
        public async Task GetAsync_IncludDetail_Should_Return_Tenant_WithDetails()
        {
            var tenantInDb = (await TenantRepository.GetListAsync(filter: "T2")).First();

            var result = await TenantExRepository.GetAsync(tx => tx.TenantId == tenantInDb.Id);
            result.ShouldNotBeNull();
            result.TenantId.ShouldBe(tenantInDb.Id);
            result.Tenant.ConnectionStrings.ShouldNotBeEmpty();
            result.Tenant.ConnectionStrings.Any().ShouldBeTrue();
            result.TenantResources.ShouldNotBeEmpty();
            result.TenantResources.Any().ShouldBeTrue();
        }

        [Fact]
        public async Task GetListAsync_WithNoFilter_ShouldReturnAllTenants()
        {
            var totalTenants = await TenantRepository.GetCountAsync();

            var results = await TenantExRepository.GetListAsync(true);
            results.Count.ShouldBe((int)totalTenants);
            results.ShouldContain(t => t.Tenant.Name == "T1");
            results.ShouldContain(t => t.Tenant.Name == "T2");
            results.ShouldContain(t => t.Tenant.Name == "T3");
            results.ShouldContain(t => t.Tenant.Name == "T4");
        }

        [Fact]
        public async Task GetListAsync_FilterByName_ShouldReturnTenantWithDetails()
        {
            var result = (await TenantExRepository.GetListAsync(filter: "T2", includeDetails: true)).First();

            var expectedConnectionString = EncryptionService.Encrypt("ConnectionStringOfT2");
            var expectedStorageConnectionString = EncryptionService.Encrypt("StorageConnectionStringOfT2");

            result.Tenant.ShouldNotBeNull();
            result.Tenant.ConnectionStrings.ShouldNotBeNull();
            result.Tenant.ConnectionStrings.Any().ShouldBeTrue();
            result.TenantResources.ShouldNotBeNull();
            result.TenantResources.ShouldContain(t => t.CloudServiceLocationId == (int)CloudServiceLocations.AustraliaEast);
            result.TenantResources.ShouldContain(t => t.CloudServiceOptionId == (int)CloudServiceOptions.DatabaseStandard);
            result.TenantResources.ShouldContain(t => t.CloudServiceOptionId == (int)CloudServiceOptions.StorageStandard);
            result.TenantResources.ShouldContain(t => t.ConnectionString == expectedConnectionString);
            result.TenantResources.ShouldContain(t => t.ConnectionString == expectedStorageConnectionString);
        }

        [Fact]
        public async Task GetCountAsync_WithNoFilter_ShouldReturnNumberOfAllTenants()
        {
            var totalTenants = await TenantRepository.GetCountAsync();
            var tenantExCount = await TenantExRepository.GetCountAsync();
            tenantExCount.ShouldBe(totalTenants);
        }

        [Fact]
        public async Task GetCountAsync_FilterTenantParentId_ShouldReturnNumberOfClients()
        {
            var clientTenant = await TenantExRepository.FindAsync(e => (e.TenantParentId != null) && (e.TenantParentId != Guid.Empty));
            var clientCount = await TenantExRepository.GetCountAsync(tenantParentId: clientTenant.TenantParentId);
            clientCount.ShouldBe(1);
        }

        [Fact]
        public async Task GetTenantListByLocationIdAsync_ShouldReturnTenantsByLocationId()
        {
            var results = await TenantExRepository.GetTenantListByLocationIdAsync((int)CloudServiceLocations.AustraliaEast);
            results.ShouldNotBeNull();
            results.Count.ShouldBe(1);
            results.ShouldContain(t => t.Name == "T2");

            var expectedConnectionString = EncryptionService.Encrypt("ConnectionStringOfT2");

            var tenant = results.First();
            tenant.ConnectionStrings.ShouldNotBeNull();
            tenant.ConnectionStrings.ShouldContain(t => t.Value == expectedConnectionString);
        }
    }
}