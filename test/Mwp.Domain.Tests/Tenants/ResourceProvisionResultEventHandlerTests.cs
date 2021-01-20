using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mwp.CloudService;
using Mwp.Provision;
using Mwp.Tenants.Events.Result;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Security.Encryption;
using Xunit;

namespace Mwp.Tenants
{
    public class ResourceProvisionResultEventHandlerTests : MwpDomainTestBase
    {
        protected IGuidGenerator GuidGenerator;
        protected ITenantExRepository TenantExRepository;
        protected IRepository<TenantResource, Guid> TenantResourceRepository;
        protected IStringEncryptionService EncryptionService;
        protected IConfiguration Config;

        readonly IDistributedEventBus _distirbutedEventBus;

        public ResourceProvisionResultEventHandlerTests()
        {
            GuidGenerator = GetRequiredService<IGuidGenerator>();
            TenantExRepository = GetRequiredService<ITenantExRepository>();
            TenantResourceRepository = GetRequiredService<IRepository<TenantResource, Guid>>();
            EncryptionService = GetRequiredService<IStringEncryptionService>();
            Config = GetRequiredService<IConfiguration>();

            _distirbutedEventBus = GetRequiredService<LocalDistributedEventBus>();
        }

        [Fact]
        public async Task WhenGot_DatabaseProvisionResultEventData_ShouldUpdateTenantResource_AndTenantConnectionString()
        {
            // arrange
            var tenantResource = await CreateExisitngTenantResource(CloudServiceOptions.DatabaseStandard);
            var provisionResult = new DatabaseProvisionResultEventData
            {
                TenantId = tenantResource.TenantId,
                ResourceId = tenantResource.Id,
                SubscriptionId = "TestSubscriptionId_Database",
                ResourceGroupName = "TestResourceGroupName_Database",
                ServerName = "TestServerName_Database",
                Name = "TestName_Database",
                StatusId = ProvisionStatus.Success
            };
            var defaultUserId = Config[TenantResourceConsts.DefaultUserIdSetting];
            var defaultPassword = Config[TenantResourceConsts.DefaultPasswordSetting];

            // act
            await _distirbutedEventBus.PublishAsync(provisionResult);

            // assert
            var updatedTenantResource = await TenantResourceRepository.GetAsync(tenantResource.Id);
            updatedTenantResource.ConnectionString.ShouldNotContain("Password="); // connectionstring should not be readable

            updatedTenantResource.ConnectionString = EncryptionService.Decrypt(updatedTenantResource.ConnectionString);
            updatedTenantResource.ShouldSatisfyAllConditions(
                () => updatedTenantResource.ConnectionString.ShouldContain($"Server={provisionResult.ServerName}.database.windows.net"),
                () => updatedTenantResource.ConnectionString.ShouldContain($"Initial Catalog={provisionResult.Name}"),
                () => updatedTenantResource.ConnectionString.ShouldContain($"User ID={defaultUserId}"),
                () => updatedTenantResource.ConnectionString.ShouldContain($"Password={defaultPassword}"),
                () => updatedTenantResource.Name.ShouldBe(provisionResult.Name),
                () => updatedTenantResource.ProvisionStatus.ShouldBe(ProvisionStatus.Success),
                () => updatedTenantResource.IsActive.ShouldBe(true));

            var tenantEx = await TenantExRepository.GetAsync(tenantResource.TenantExId);
            var tenantConnectionString = EncryptionService.Decrypt(tenantEx.Tenant.ConnectionStrings.First().Value);

            tenantEx.IsActive.ShouldBe(true);
            tenantConnectionString.ShouldSatisfyAllConditions(
                () => tenantConnectionString.ShouldContain($"Server={provisionResult.ServerName}.database.windows.net"),
                () => tenantConnectionString.ShouldContain($"Initial Catalog={provisionResult.Name}"));
        }

        [Fact]
        public async Task WhenGot_StorageProvisionResultEventData_ShouldUpdateTenantResource()
        {
            // arrange
            var tenantResource = await CreateExisitngTenantResource(CloudServiceOptions.StoragePremium);

            var provisionResult = new StorageProvisionResultEventData
            {
                TenantId = tenantResource.TenantId,
                ResourceId = tenantResource.Id,
                SubscriptionId = "TestSubscriptionId_Storage",
                ResourceGroupName = "TestResourceGroupName_Storage",
                ServerName = "TestServerName_Storage",
                Name = "TestName_Storage",
                StatusId = ProvisionStatus.Success,
                ConnectionString = "StorageConnectionStringFromProvision"
            };

            // act
            await _distirbutedEventBus.PublishAsync(provisionResult);

            // assert
            var updatedTenantResource = await TenantResourceRepository.GetAsync(tenantResource.Id);

            updatedTenantResource.ShouldSatisfyAllConditions(
                () => updatedTenantResource.ConnectionString.ShouldNotContain("AccountName="),
                () => updatedTenantResource.ConnectionString.ShouldBe(EncryptionService.Encrypt(provisionResult.ConnectionString)),
                () => updatedTenantResource.Name.ShouldBe(provisionResult.Name),
                () => updatedTenantResource.ProvisionStatus.ShouldBe(ProvisionStatus.Success),
                () => updatedTenantResource.IsActive.ShouldBe(true));
        }

        #region methods

        async Task<TenantResource> CreateExisitngTenantResource(CloudServiceOptions cloudServiceOption)
        {
            var tenantEx = await GetDefaultTenantEx();

            var resource = new TenantResource(tenantEx.TenantId, tenantEx.Id)
            {
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)cloudServiceOption
            };

            await TenantResourceRepository.InsertAsync(resource, true);

            return resource;
        }

        async Task<TenantEx> GetDefaultTenantEx()
        {
            var tenants = await TenantExRepository.GetListAsync();
            var defaultTenantEx = tenants.First();
            return defaultTenantEx;
        }

        #endregion
    }
}