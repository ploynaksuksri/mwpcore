using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mwp.CloudService;
using Mwp.Extensions;
using Mwp.Provision;
using Mwp.SharedResource;
using Mwp.SqlDatabase;
using Mwp.Storage;
using Mwp.Tenants.Dtos;
using Mwp.Users;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Saas.Host.Dtos;
using Volo.Saas.Tenants;
using Xunit;

namespace Mwp.Tenants
{
    public class MwpTenantAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipReasonForManualRun = "Skipped because the process is time consuming and supposed to be run manually only.";

        public const string Password = "abc123*X";

        readonly IMwpTenantAppService MwpTenantAppService;
        readonly ITenantRepository TenantRepository;
        readonly IRepository<TenantResource> TenantResourceRepository;
        readonly ITenantExRepository TenantExRepository;
        readonly ICurrentTenant CurrentTenant;
        readonly ISharedResourceRepository SharedResourceRepository;
        readonly IGuidGenerator GuidGenerator;
        readonly IStorageManager AzureStorageManager;
        readonly IDatabaseManager AzureDatabaseManager;
        readonly IRepository<AppUser> UserRepository;
        readonly IStringEncryptionService EncryptionService;
        readonly IJsonSerializer Json;
        readonly IConfiguration Config;

        public MwpTenantAppServiceTests()
        {
            MwpTenantAppService = GetRequiredService<IMwpTenantAppService>();
            TenantRepository = GetRequiredService<ITenantRepository>();
            TenantResourceRepository = GetRequiredService<IRepository<TenantResource>>();
            TenantExRepository = GetRequiredService<ITenantExRepository>();
            CurrentTenant = GetRequiredService<ICurrentTenant>();
            SharedResourceRepository = GetRequiredService<ISharedResourceRepository>();
            GuidGenerator = GetRequiredService<IGuidGenerator>();
            AzureStorageManager = GetRequiredService<IStorageManager>();
            AzureDatabaseManager = GetRequiredService<IDatabaseManager>();
            UserRepository = GetRequiredService<IRepository<AppUser>>();
            EncryptionService = GetRequiredService<IStringEncryptionService>();
            Json = GetRequiredService<IJsonSerializer>();
            Config = GetRequiredService<IConfiguration>();
        }

        [Fact]
        [Trait("Category", "Create Tenant")]
        public async Task CreateTenantMwp_WithBasicDatabase_StandardStorage_ShouldGetTenantResourceRecord()
        {
            // arrange

            var newTenant = new MwpSaasTenantCreateDto
            {
                LocationId = (int)CloudServiceLocations.AustraliaEast,
                DatabaseOptionId = (int)CloudServiceOptions.DatabaseBasic,
                StorageOptionId = (int)CloudServiceOptions.StorageStandard,
                AdminEmailAddress = "tenantadmin@mwp.com",
                AdminPassword = Password,
                EditionId = Guid.NewGuid(),
                Name = "My tenant 1"
            };

            var expectedDatabase = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.DatabaseBasic,
                ProvisionStatus = ProvisionStatus.Success,
                IsActive = true
            };

            var expectedStorage = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.StorageStandard,
                ProvisionStatus = ProvisionStatus.Success,
                IsActive = true
            };

            // act
            var result = await WithUnitOfWorkAsync(async () => await MwpTenantAppService.CreateAsync(newTenant));

            // assert
            result.Id.ShouldNotBe(Guid.Empty);

            var tenant = await TenantExRepository.GetAsync(tx => tx.TenantId == result.Id);

            AssertTenant(tenant, newTenant, expectedDatabase.IsActive, null);
            await AssertTenantResources(tenant, newTenant, expectedDatabase, expectedStorage);
        }

        [Fact(Skip = SkipReasonForManualRun)]
        [Trait("Category", "Create Tenant")]
        public async Task CreateTenantMwp_WithPremiumDatabase_StandardStorage_ShouldProvisionResources()
        {
            // arrange
            var newTenant = new MwpSaasTenantCreateDto
            {
                LocationId = (int)CloudServiceLocations.AustraliaEast,
                DatabaseOptionId = (int)CloudServiceOptions.DatabasePremium,
                StorageOptionId = (int)CloudServiceOptions.StorageStandard,
                AdminEmailAddress = "admin@mwp.com",
                AdminPassword = Password,
                EditionId = Guid.NewGuid(),
                Name = "Premium tenant " + new Random().Next(1000, 5000)
            };

            var expectedDatabase = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.DatabasePremium,
                ProvisionStatus = ProvisionStatus.Success,
                IsActive = true
            };

            var expectedStorage = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.StorageStandard,
                ProvisionStatus = ProvisionStatus.Success,
                IsActive = true
            };

            // act
            var result = await WithUnitOfWorkAsync(async () => await MwpTenantAppService.CreateAsync(newTenant));

            // assert
            result.Id.ShouldNotBe(Guid.Empty);

            var tenant = await TenantExRepository.GetAsync(tx => tx.TenantId == result.Id);

            AssertTenant(tenant, newTenant, expectedDatabase.IsActive, null);
            await AssertTenantResources(tenant, newTenant, expectedDatabase, expectedStorage);

            var databaseTenantResource = await TenantResourceRepository.GetAsync(e => e.TenantId == result.Id && e.CloudServiceOptionId == expectedDatabase.CloudServiceOptionId, false);
            var isDatabaseServerNameAvailable = await AzureDatabaseManager.CheckServerNameAvailabilityAsync(databaseTenantResource.ServerName);
            isDatabaseServerNameAvailable.ShouldBeFalse();
            await AzureDatabaseManager.DeleteAsync(databaseTenantResource.ResourceGroup, databaseTenantResource.ServerName);
        }

        [Fact(Skip = SkipReasonForManualRun)]
        [Trait("Category", "Create Tenant")]
        public async Task CreateTenantMwp_WithStandardDatabase_PremiumStorage_ShouldProvisionResources()
        {
            // arrange
            var newTenant = new MwpSaasTenantCreateDto
            {
                LocationId = (int)CloudServiceLocations.AustraliaEast,
                DatabaseOptionId = (int)CloudServiceOptions.DatabaseStandard,
                StorageOptionId = (int)CloudServiceOptions.StoragePremium,
                AdminEmailAddress = "admin@mwp.com",
                AdminPassword = Password,
                EditionId = Guid.NewGuid(),
                Name = "My tenant" + new Random().Next(1000, 5000)
            };

            var expectedDatabase = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.DatabaseStandard,
                ProvisionStatus = ProvisionStatus.Success,
                IsActive = true
            };

            var expectedStorage = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.StoragePremium,
                ProvisionStatus = ProvisionStatus.Success,
                IsActive = true
            };

            var secret = Json.Deserialize<SharedResourceSecret>(Config[SharedResourceSecrets.Aue.DatabaseStandard]);
            var sqlServer = await AzureDatabaseManager.CreateSqlServer(secret.ResourceGroup, secret.Key, (CloudServiceLocations)newTenant.LocationId);
            await AzureDatabaseManager.CreateElasticPool(sqlServer.ResourceGroupName, sqlServer.Name, secret.Name);

            // act
            var result = await WithUnitOfWorkAsync(async () => await MwpTenantAppService.CreateAsync(newTenant));

            // assert
            result.Id.ShouldNotBe(Guid.Empty);

            var tenant = await TenantExRepository.GetAsync(tx => tx.TenantId == result.Id);

            AssertTenant(tenant, newTenant, expectedDatabase.IsActive, null);
            await AssertTenantResources(tenant, newTenant, expectedDatabase, expectedStorage);

            var storageTenantResource = await TenantResourceRepository.GetAsync(e => e.TenantId == result.Id && e.CloudServiceOptionId == expectedStorage.CloudServiceOptionId, false);
            var isStorageNameAvailable = !await AzureDatabaseManager.CheckServerNameAvailabilityAsync(storageTenantResource.Name);
            isStorageNameAvailable.ShouldBeFalse();
            await AzureStorageManager.DeleteAsync(storageTenantResource.ResourceGroup, storageTenantResource.Name);
            await AzureDatabaseManager.DeleteAsync(secret.ResourceGroup, secret.Key);
        }

        [Fact]
        [Trait("Category", "Create Tenant")]
        public async Task CreateTenantMwp_WithParentId_ShouldSaveCorrectParentId()
        {
            // arrange
            var firstTenant = await TenantRepository.FindByNameAsync("T1");
            var newTenant = new MwpSaasTenantCreateDto
            {
                LocationId = (int)CloudServiceLocations.AustraliaEast,
                DatabaseOptionId = (int)CloudServiceOptions.DatabaseBasic,
                StorageOptionId = (int)CloudServiceOptions.StorageStandard,
                AdminEmailAddress = "admin@mwp.com",
                AdminPassword = Password,
                EditionId = Guid.NewGuid(),
                Name = "My tenant 1"
            };

            var expectedDatabase = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.DatabaseBasic,
                ProvisionStatus = ProvisionStatus.Success,
                IsActive = true
            };
            var expectedStorage = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.StorageStandard,
                ProvisionStatus = ProvisionStatus.Success,
                IsActive = true
            };

            // act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                using (CurrentTenant.Change(firstTenant.Id))
                {
                    return await MwpTenantAppService.CreateAsync(newTenant);
                }
            });

            // assert
            result.Id.ShouldNotBe(Guid.Empty);

            var tenant = await TenantExRepository.GetAsync(tx => tx.TenantId == result.Id);

            AssertTenant(tenant, newTenant, expectedDatabase.IsActive, firstTenant.Id);

            await AssertTenantResources(tenant, newTenant, expectedDatabase, expectedStorage);
        }

        [Fact]
        [Trait("Category", "Create Tenant")]
        public async Task CreateTenantMwp_WithInvalidLocationId_Should_ThrowError()
        {
            // arrange
            var newTenant = new MwpSaasTenantCreateDto
            {
                LocationId = 99,
                DatabaseOptionId = (int)CloudServiceOptions.DatabaseStandard,
                StorageOptionId = (int)CloudServiceOptions.StorageStandard,
                AdminEmailAddress = "admin@mwp.com",
                AdminPassword = Password,
                EditionId = Guid.NewGuid(),
                Name = "My tenant"
            };

            // act
            var ex = await Assert.ThrowsAsync<UserFriendlyException>(() => MwpTenantAppService.CreateAsync(newTenant));

            // assert
            Assert.Equal("Selected cloud service location is not available.", ex.Message);
        }

        [Fact]
        [Trait("Category", "Create Tenant")]
        public async Task CreateTenantMwp_WithInvalidDatabaseOption_Should_ThrowError()
        {
            // arrange
            var newTenant = new MwpSaasTenantCreateDto
            {
                LocationId = (int)CloudServiceLocations.UKSouth,
                DatabaseOptionId = 99,
                StorageOptionId = (int)CloudServiceOptions.StorageStandard,
                AdminEmailAddress = "admin@mwp.com",
                AdminPassword = Password,
                EditionId = Guid.NewGuid(),
                Name = "My tenant"
            };

            var ex = await Assert.ThrowsAsync<UserFriendlyException>(() => MwpTenantAppService.CreateAsync(newTenant));

            Assert.Equal("Selected cloud service option is not available.", ex.Message);
        }

        [Fact]
        [Trait("Category", "Create Tenant")]
        public async Task CreateTenantMwp_WithInvalidStorageOption_Should_ThrowError()
        {
            // arrange
            var newTenant = new MwpSaasTenantCreateDto
            {
                LocationId = (int)CloudServiceLocations.UKSouth,
                DatabaseOptionId = (int)CloudServiceOptions.DatabaseStandard,
                StorageOptionId = 99,
                AdminEmailAddress = "admin@mwp.com",
                AdminPassword = Password,
                EditionId = Guid.NewGuid(),
                Name = "My tenant"
            };

            // act
            var ex = await Assert.ThrowsAsync<UserFriendlyException>(() => MwpTenantAppService.CreateAsync(newTenant));

            // assert
            Assert.Equal("Selected cloud service option is not available.", ex.Message);
        }

        [Fact]
        [Trait("Category", "CRUD")]
        public async Task GetListAsync_WhenLoginAsHost_Should_Return_AllTenants()
        {
            var countTenants = await TenantRepository.GetCountAsync();

            var result = await WithUnitOfWorkAsync(async () =>
            {
                using (CurrentTenant.Change(null))
                {
                    return await MwpTenantAppService.GetListAsync(new GetTenantsInput());
                }
            });
            result.TotalCount.ShouldBe(countTenants);
            result.Items.ShouldContain(t => t.Name == "T1");
            result.Items.ShouldContain(t => t.Name == "T2");
            result.Items.ShouldContain(t => t.Name == "T3");
            result.Items.ShouldContain(t => t.Name == "T4");
        }

        [Fact]
        [Trait("Category", "CRUD")]
        public async Task GetListAsync_WhenLoginAsTenant_Should_Return_OnlyTheirClients()
        {
            var clientTenant = await TenantExRepository.FindAsync(e => (e.TenantParentId != null) && (e.TenantParentId != Guid.Empty));
            var countClientTenants = await TenantExRepository.GetCountAsync(tenantParentId: clientTenant.TenantParentId);

            var result = await WithUnitOfWorkAsync(async () =>
            {
                using (CurrentTenant.Change(clientTenant.TenantParentId))
                {
                    return await MwpTenantAppService.GetListAsync(new GetTenantsInput());
                }
            });

            result.TotalCount.ShouldBe(countClientTenants);
            result.Items.ShouldContain(t => t.Name == clientTenant.Tenant.Name);

            var tenant2 = result.Items.FirstOrDefault(i => i.Id == clientTenant.TenantId);
            if (tenant2 != null)
            {
                tenant2.LocationId.ShouldBe((int)CloudServiceLocations.AustraliaEast);
                tenant2.DatabaseOptionId.ShouldBe((int)CloudServiceOptions.DatabaseStandard);
                tenant2.StorageOptionId.ShouldBe((int)CloudServiceOptions.StorageStandard);
                tenant2.LocationName.ShouldBe(CloudServiceLocations.AustraliaEast.GetDescription());
                tenant2.DatabaseOptionName.ShouldBe(CloudServiceOptions.DatabaseStandard.GetName());
                tenant2.StorageOptionName.ShouldBe(CloudServiceOptions.StorageStandard.GetName());
            }
        }

        [Fact]
        [Trait("Category", "CRUD")]
        public async Task GetAsync_Should_Return_Tenant_WithResourceInfo()
        {
            var tenantInDb = await TenantRepository.FindByNameAsync("T2");

            var result = await MwpTenantAppService.GetAsync(tenantInDb.Id);
            result.Name.ShouldBe("T2");

            result.LocationId.ShouldBe((int)CloudServiceLocations.AustraliaEast);
            result.DatabaseOptionId.ShouldBe((int)CloudServiceOptions.DatabaseStandard);
            result.StorageOptionId.ShouldBe((int)CloudServiceOptions.StorageStandard);
            result.LocationName.ShouldBeNullOrEmpty();
            result.DatabaseOptionName.ShouldBeNullOrEmpty();
            result.StorageOptionName.ShouldBeNullOrEmpty();
        }

        [Fact]
        [Trait("Category", "CRUD")]
        public async Task UpdateAsync_Should_Update_MwpTenant()
        {
            var tenantInDb = await TenantRepository.FindByNameAsync("T1");
            var input = new SaasTenantUpdateDto
            {
                EditionId = GuidGenerator.Create(),
                Name = "New Tenant"
            };
            var result = await MwpTenantAppService.UpdateAsync(tenantInDb.Id, input);
            result.Name.ShouldBe(input.Name);
        }

        [Fact]
        [Trait("Category", "CRUD")]
        public async Task UpdateAndGetDefaultConnectionStringAsync_Should_Success()
        {
            // Update
            var tenantInDb = await TenantRepository.FindByNameAsync("T2");
            var newConnectionString = $"New{tenantInDb.FindDefaultConnectionString()}";

            await MwpTenantAppService.UpdateDefaultConnectionStringAsync(tenantInDb.Id, newConnectionString);

            var updatedTenant = await TenantRepository.GetAsync(tenantInDb.Id);
            var defaultConnectionString = updatedTenant.FindDefaultConnectionString();
            defaultConnectionString.ShouldNotBeNullOrEmpty();
            defaultConnectionString.ShouldNotContain("Password=");
            EncryptionService.Decrypt(defaultConnectionString).ShouldBe(newConnectionString);

            // Get
            defaultConnectionString = await MwpTenantAppService.GetDefaultConnectionStringAsync(tenantInDb.Id);
            defaultConnectionString.ShouldNotBeNullOrEmpty();
            defaultConnectionString.ShouldBe(newConnectionString);
        }

        [Fact]
        [Trait("Category", "CRUD")]
        public async Task DeleteAsync_Should_Delete_ById()
        {
            var tenantInDb = await TenantRepository.FindByNameAsync("T3");

            await MwpTenantAppService.DeleteAsync(tenantInDb.Id);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => MwpTenantAppService.GetAsync(tenantInDb.Id));
            await Assert.ThrowsAsync<EntityNotFoundException>(() => TenantExRepository.GetByTenantIdAsync(tenantInDb.Id));
        }

        #region Private methods

        void AssertTenant(TenantEx tenantEx, MwpSaasTenantCreateDto tenantRequest, bool? expectedIsActive, Guid? expectedTenantParentId)
        {
            tenantEx.IsActive.ShouldBe(expectedIsActive);
            tenantEx.TenantParentId.ShouldBe(expectedTenantParentId);
            tenantEx.Tenant.Name.ShouldBe(tenantRequest.Name);
            tenantEx.Tenant.EditionId.ShouldBe(tenantRequest.EditionId);

            using (CurrentTenant.Change(tenantEx.TenantId))
            {
                var user = UserRepository.GetAsync(e => e.TenantId == tenantEx.TenantId && e.UserName == "admin").GetAwaiter().GetResult();
                user.Email.ShouldBe(tenantRequest.AdminEmailAddress);
            }
        }

        async Task AssertTenantResources(TenantEx tenantEx, MwpSaasTenantCreateDto tenantDto, TenantResource expectedDatabase, TenantResource expectedStorage)
        {
            var tenant = await TenantRepository.GetAsync(tenantEx.TenantId);
            var tenantResources = (await TenantResourceRepository.GetListAsync())
                .Where(e => e.TenantId == tenant.Id)
                .ToList();

            // Verify database resource
            var databaseTenantResource = tenantResources.First(e => e.CloudServiceOptionId == tenantDto.DatabaseOptionId);
            await AssertTenantResource(databaseTenantResource, expectedDatabase);
            // Verify connection string on SaasConnectionString
            databaseTenantResource.ConnectionString.ShouldBe(tenant.FindDefaultConnectionString());

            // Verify storage resource
            var storageTenantResource = tenantResources.FirstOrDefault(e => e.CloudServiceOptionId == tenantDto.StorageOptionId);
            await AssertTenantResource(storageTenantResource, expectedStorage);
        }

        async Task AssertTenantResource(TenantResource result, TenantResource expected)
        {
            var sharedResource = await SharedResourceRepository.GetSharedResourceByOptionId(result.CloudServiceOptionId, result.CloudServiceLocationId);
            var sharedResourceSecret = GetSharedResourceSecret(sharedResource.SecretName);

            result.IsActive.ShouldBe(expected.IsActive);
            result.ProvisionStatus.ShouldBe(expected.ProvisionStatus);

            // Verify that connectionstring is encrypted
            result.ConnectionString.ShouldNotBeNull();
            result.ConnectionString.ShouldNotContain("Password=");
            result.ConnectionString.ShouldNotContain("AccountName=");

            if (!sharedResource.CloudServiceOption.IsProvisionRequired)
            {
                EncryptionService.Decrypt(result.ConnectionString).ShouldBe(sharedResourceSecret.Key);
            }
        }

        private SharedResourceSecret GetSharedResourceSecret(string secretName)
        {
            return Json.Deserialize<SharedResourceSecret>(Config[secretName]);
        }

        #endregion Private methods
    }
}