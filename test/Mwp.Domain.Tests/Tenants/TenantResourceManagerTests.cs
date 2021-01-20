using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mwp.CloudService;
using Mwp.Tenants.Events.Request;
using NSubstitute;
using Shouldly;
using Volo.Abp.Json;
using Volo.Abp.Security.Encryption;
using Xunit;

namespace Mwp.Tenants
{
    /// <summary>
    ///     To verify that TenantResourceManager can provide valid tenant records per options in request
    /// </summary>
    public class TenantResourceManagerTests : MwpDomainTestBase
    {
        protected readonly ITenantResourceManager TenantResourceManager;
        protected readonly ITenantExRepository TenantExRepository;
        protected readonly ISharedResourceRepository SharedResourceRepository;
        protected readonly IStringEncryptionService EncryptionService;
        protected readonly IConfiguration Config;
        protected readonly IJsonSerializer Json;

        public TenantResourceManagerTests()
        {
            TenantExRepository = GetRequiredService<ITenantExRepository>();
            SharedResourceRepository = GetRequiredService<ISharedResourceRepository>();
            EncryptionService = GetRequiredService<IStringEncryptionService>();
            TenantResourceManager = GetRequiredService<ITenantResourceManager>();
            Config = GetRequiredService<IConfiguration>();
            Json = GetRequiredService<IJsonSerializer>();
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            var provisionRequestManager = Substitute.For<IResourceProvisionRequestManager>();
            provisionRequestManager.ProvisionDatabase(new TenantResource(Guid.NewGuid(), Guid.NewGuid()));
            provisionRequestManager.ProvisionStorage(new TenantResource(Guid.NewGuid(), Guid.NewGuid()));
            services.AddSingleton(provisionRequestManager);
        }

        [Theory]
        [InlineData(CloudServiceOptions.DatabaseBasic)]
        [InlineData(CloudServiceOptions.DatabaseStandard)]
        public async Task ProvideTenantDatabase_WithBasic_AndStandardOptions_ShouldGetTenantResourceRecord(
            CloudServiceOptions cloudServiceOption)
        {
            var defaultTenantEx = await GetDefaultTenantEx();
            var request = new TenantResourceRequest
            {
                DatabaseOptionId = (int)cloudServiceOption,
                StorageOptionId = (int)CloudServiceOptions.StorageStandard,
                LocationId = (int)CloudServiceLocations.AustraliaEast,
                TenantId = defaultTenantEx.TenantId,
                TenantExId = defaultTenantEx.Id,
                TenantName = "My Tenant"
            };

            var sharedResource = await SharedResourceRepository.GetSharedResourceByOptionId(request.DatabaseOptionId, request.LocationId);
            var expected = GetSharedResourceSecret(sharedResource.SecretName);
            var result = await WithUnitOfWorkAsync(async () => await TenantResourceManager.ProvideTenantDatabase(request)); 

            result.Id.ShouldNotBe(Guid.Empty);
            result.TenantId.ShouldBe(request.TenantId);
            result.SubscriptionId.ShouldBe(expected.SubscriptionId);
            result.ResourceGroup.ShouldBe(expected.ResourceGroup);

            if (sharedResource.CloudServiceOption.IsProvisionRequired)
            {
                result.ConnectionString.ShouldBeNull();
            }
            else
            {
                result.ConnectionString.ShouldBe(EncryptionService.Encrypt(expected.Key));
                result.Name.ShouldBe(expected.Name);
            }
        }

        [Theory]
        [InlineData(CloudServiceOptions.StorageStandard)]
        [InlineData(CloudServiceOptions.StoragePremium)]
        public async Task ProvideTenantStorage_WithStandard_AndPremiumOptions_ShouldGetTenantResourceRecord(CloudServiceOptions cloudServiceOption)
        {
            var defaultTenantEx = await GetDefaultTenantEx();
            var request = new TenantResourceRequest
            {
                DatabaseOptionId = (int)CloudServiceOptions.DatabaseBasic,
                StorageOptionId = (int)cloudServiceOption,
                LocationId = (int)CloudServiceLocations.AustraliaEast,
                TenantId = defaultTenantEx.TenantId,
                TenantExId = defaultTenantEx.Id,
                TenantName = "My Tenant"
            };

            var sharedResource = await SharedResourceRepository.GetSharedResourceByOptionId(request.StorageOptionId, request.LocationId);
            var expected = GetSharedResourceSecret(sharedResource.SecretName);
            var encryptedKey = EncryptionService.Encrypt(expected.Key);
            var result = await TenantResourceManager.ProvideTenantStorage(request);

            result.Id.ShouldNotBe(Guid.Empty);
            result.TenantId.ShouldBe(request.TenantId);
            result.SubscriptionId.ShouldBe(expected.SubscriptionId);
            result.ResourceGroup.ShouldBe(expected.ResourceGroup);

            if (sharedResource.CloudServiceOption.IsProvisionRequired)
            {
                result.ConnectionString.ShouldBeNull();
            }
            else
            {
                result.ConnectionString.ShouldBe(encryptedKey);
                result.Name.ShouldBe(expected.Name);
            }
        }

        internal async Task<TenantEx> GetDefaultTenantEx()
        {
            var tenants = await TenantExRepository.GetListAsync();
            return tenants.First();
        }

        private SharedResourceSecret GetSharedResourceSecret(string secretName)
        {
            return Json.Deserialize<SharedResourceSecret>(Config[secretName]);
        }
    }
}