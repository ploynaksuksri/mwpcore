using System.Linq;
using System.Threading.Tasks;
using Mwp.CloudService;
using Mwp.Provision;
using Mwp.SqlDatabase;
using Mwp.Storage;
using Mwp.Tenants.Events.Request;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Encryption;
using Xunit;

namespace Mwp.Tenants
{
    public class ResourceSDKProvisionRequestManagerTests : MwpDomainTestBase
    {
        public const string SkipReasonForManualRun = "Skipped because the process is time consuming.";
        protected IResourceProvisionRequestManager ProvisionRequestManager;
        protected IStorageManager AzureStorageRM;
        protected IDatabaseManager AzureDatabaseRM;
        protected IRepository<TenantResource> TenantResourceRepository;
        protected ITenantExRepository TenantExRepository;
        protected IStringEncryptionService EncryptionService;

        protected readonly CloudServiceLocations _defaultAzureRegion = CloudServiceLocations.SoutheastAsia;
        protected readonly string _databaseName = "tenantdata";
        protected readonly string _resourceGroupName = "mwp-local-app";
        protected readonly string _subscriptionId = "29c044a6-2a71-48fb-9cb0-b4fdbe7c878a";

        private readonly TenantEx _defaultTenantEx;

        public ResourceSDKProvisionRequestManagerTests()
        {
            ProvisionRequestManager = GetRequiredService<IResourceProvisionRequestManager>();
            AzureStorageRM = GetRequiredService<IStorageManager>();
            AzureDatabaseRM = GetRequiredService<IDatabaseManager>();
            TenantResourceRepository = GetRequiredService<IRepository<TenantResource>>();
            TenantExRepository = GetRequiredService<ITenantExRepository>();
            EncryptionService = GetRequiredService<IStringEncryptionService>();

            _defaultTenantEx = GetDefaultTenantEx().Result;
        }

        [Fact(Skip = SkipReasonForManualRun)]
        public async Task Should_Successfully_Create_StandardDatabase()
        {
            var resource = new TenantResource(_defaultTenantEx.TenantId, _defaultTenantEx.Id)
            {
                Name = _databaseName,
                IsActive = false,
                CloudServiceLocationId = (int)_defaultAzureRegion,
                CloudServiceOptionId = (int)CloudServiceOptions.DatabaseStandard,
                ServerName = "db-test-pool",
                ElasticPoolName = "test-pool",
                ResourceGroup = _resourceGroupName,
                SubscriptionId = _subscriptionId
            };
            await TenantResourceRepository.InsertAsync(resource);
            var sqlServer = await AzureDatabaseRM.CreateSqlServer(resource.ResourceGroup, resource.ServerName, (CloudServiceLocations)resource.CloudServiceLocationId);
            await AzureDatabaseRM.CreateElasticPool(sqlServer.ResourceGroupName, sqlServer.Name, resource.ElasticPoolName);

            // Act
            await ProvisionRequestManager.ProvisionDatabase(resource);

            // Assert
            var database = await AzureDatabaseRM.GetDatabase(resource.ResourceGroup, resource.ServerName, resource.Name);
            database.ElasticPoolName.ShouldBe(resource.ElasticPoolName);
            var updatedResource = await TenantResourceRepository.GetAsync(e => e.Id == resource.Id, false);
            updatedResource.ProvisionStatus.ShouldBe(ProvisionStatus.Success);
            updatedResource.ConnectionString.ShouldNotContain($"Initial Catalog={updatedResource.Name}");
            EncryptionService.Decrypt(updatedResource.ConnectionString).ShouldContain($"Initial Catalog={updatedResource.Name}");

            // Delete resources
            await AzureDatabaseRM.DeleteAsync(resource.ResourceGroup, resource.ServerName);
        }

        [Fact(Skip = SkipReasonForManualRun)]
        public async Task Should_Successfully_Create_PremiumDatabase()
        {
            var resource = new TenantResource(_defaultTenantEx.TenantId, _defaultTenantEx.Id)
            {
                Name = _databaseName,
                IsActive = false,
                CloudServiceLocationId = (int)_defaultAzureRegion,
                CloudServiceOptionId = (int)CloudServiceOptions.DatabasePremium,
                ServerName = "premiumdbtest",
                ResourceGroup = _resourceGroupName,
                SubscriptionId = _subscriptionId,
                AdminEmailAddress = "admin@mwp.com",
                AdminPassword = "1q2w3E*"
            };
            await TenantResourceRepository.InsertAsync(resource);

            // Act
            await ProvisionRequestManager.ProvisionDatabase(resource);

            // Assert
            var database = await AzureDatabaseRM.GetDatabase(resource.ResourceGroup, resource.ServerName, resource.Name);
            database.ShouldNotBeNull();
            var updatedResource = await TenantResourceRepository.GetAsync(e => e.Id == resource.Id, false);
            updatedResource.ProvisionStatus.ShouldBe(ProvisionStatus.Success);
            updatedResource.ConnectionString.ShouldNotContain($"Initial Catalog={updatedResource.Name}");
            EncryptionService.Decrypt(updatedResource.ConnectionString).ShouldContain($"Initial Catalog={updatedResource.Name}");

            // Delete resource
            await AzureDatabaseRM.DeleteAsync(resource.ResourceGroup, resource.ServerName);
        }

        [Fact(Skip = SkipReasonForManualRun)]
        public async Task Should_Fail_To_Create_PremiumDatabase_If_ServerName_Exists()
        {
            var resource = new TenantResource(_defaultTenantEx.TenantId, _defaultTenantEx.Id)
            {
                Name = "premiumdb",
                IsActive = false,
                CloudServiceLocationId = (int)_defaultAzureRegion,
                CloudServiceOptionId = (int)CloudServiceOptions.DatabasePremium,
                ServerName = "dev-com-dbsv",
                ResourceGroup = "mwp-dev-com",
                SubscriptionId = _subscriptionId
            };
            await TenantResourceRepository.InsertAsync(resource);

            // Act
            await ProvisionRequestManager.ProvisionDatabase(resource);

            // Assert
            var updatedResource = await TenantResourceRepository.GetAsync(e => e.Id == resource.Id, false);
            updatedResource.ProvisionStatus.ShouldBe(ProvisionStatus.Fail);
        }

        [Fact(Skip = SkipReasonForManualRun)]
        public async Task Should_Successfully_Create_PremiumStorage()
        {
            var resource = new TenantResource(_defaultTenantEx.TenantId, _defaultTenantEx.Id)
            {
                Name = "mwpunitteststg",
                IsActive = false,
                CloudServiceLocationId = (int)_defaultAzureRegion,
                CloudServiceOptionId = (int)CloudServiceOptions.StoragePremium,
                ResourceGroup = _resourceGroupName,
                SubscriptionId = _subscriptionId
            };
            await TenantResourceRepository.InsertAsync(resource);

            // Act
            await ProvisionRequestManager.ProvisionStorage(resource);

            // Assert
            var isResourceCreated = !await AzureStorageRM.CheckAccountNameAvailability(resource.Name);
            isResourceCreated.ShouldBeTrue();
            var updatedResource = await TenantResourceRepository.GetAsync(e => e.Id == resource.Id, false);
            updatedResource.ProvisionStatus.ShouldBe(ProvisionStatus.Success);
            updatedResource.ConnectionString.ShouldNotContain($"AccountName={updatedResource.Name}");
            EncryptionService.Decrypt(updatedResource.ConnectionString).ShouldContain($"AccountName={updatedResource.Name}");

            // Delete resource
            await AzureStorageRM.DeleteAsync(resource.ResourceGroup, resource.Name);
        }

        [Fact(Skip = SkipReasonForManualRun)]
        public async Task Should_Fail_To_Create_PremiumStorage_If_Name_Exists()
        {
            var resource = new TenantResource(_defaultTenantEx.TenantId, _defaultTenantEx.Id)
            {
                Name = "devcomstrg",
                IsActive = false,
                CloudServiceLocationId = (int)_defaultAzureRegion,
                CloudServiceOptionId = (int)CloudServiceOptions.StoragePremium,
                ResourceGroup = "mwp-dev-com",
                SubscriptionId = _subscriptionId
            };
            await TenantResourceRepository.InsertAsync(resource);

            // Act
            await ProvisionRequestManager.ProvisionStorage(resource);

            // Assert
            var updatedResource = await TenantResourceRepository.GetAsync(e => e.Id == resource.Id, false);
            updatedResource.ProvisionStatus.ShouldBe(ProvisionStatus.Fail);
        }

        async Task<TenantEx> GetDefaultTenantEx()
        {
            var tenants = await TenantExRepository.GetListAsync();
            return tenants.First();
        }
    }
}