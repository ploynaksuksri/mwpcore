using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mwp.CloudService;
using Mwp.Settings;
using Mwp.Tenants.Dtos;
using Mwp.Users;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Abp.Settings;
using Volo.Saas.Tenants;
using Xunit;

namespace Mwp.Tenants
{
    public class MwpTenantAppServiceFreeTrialTests : MwpApplicationTestBase
    {
        public const string SkipReasonForManualRun = "Skipped because the process is time consuming and supposed to be run manually only.";

        private readonly IMwpTenantAppService _mwpTenantAppService;
        private readonly ITenantRepository _tenantRepository;
        private readonly ISharedResourceRepository _sharedResourceRepository;
        private readonly IRepository<TenantResource> _tenantResourceRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly IRepository<AppUser> _userRepository;
        private readonly IStringEncryptionService _encryptionService;
        private readonly IConfiguration _config;
        private readonly IJsonSerializer _json;

        private const string _defaultAdminPassword = "abc123*X";

        public MwpTenantAppServiceFreeTrialTests()
        {
            _mwpTenantAppService = GetRequiredService<IMwpTenantAppService>();
            _tenantRepository = GetRequiredService<ITenantRepository>();
            _sharedResourceRepository = GetRequiredService<ISharedResourceRepository>();
            _tenantResourceRepository = GetRequiredService<IRepository<TenantResource>>();
            _currentTenant = GetRequiredService<ICurrentTenant>();
            _userRepository = GetRequiredService<IRepository<AppUser>>();
            _encryptionService = GetRequiredService<IStringEncryptionService>();
            _config = GetRequiredService<IConfiguration>();
            _json = GetRequiredService<IJsonSerializer>();

            var configuration = GetRequiredService<IConfiguration>();
            configuration[$"{ConfigurationSettingValueProvider.ConfigurationNamePrefix}{MwpSettings.IsFreeTrial}"] = "true";
        }

        [Fact(Skip = SkipReasonForManualRun)]
        public async Task CreateTenantMwp_FreeTrial_WithNullOptions_ShouldGetDefaultResources()
        {
            // arrange
            var startTime = DateTime.Now;

            var newTenant = new MwpSaasTenantCreateDto
            {
                LocationId = (int)CloudServiceLocations.AustraliaEast,
                DatabaseOptionId = null,
                StorageOptionId = null,
                AdminEmailAddress = "tenantadmin@mwp.com",
                AdminPassword = _defaultAdminPassword,
                EditionId = Guid.NewGuid(),
                Name = "My tenant"
            };

            // act
            var result = await WithUnitOfWorkAsync(() => _mwpTenantAppService.CreateAsync(newTenant));

            // assert
            var expectedDatabaseConnectionString = await GetSharedResourceKey((int)CloudServiceOptions.DatabaseBasic, (int)CloudServiceLocations.AustraliaEast);
            var expectedStorageConnectionString = await GetSharedResourceKey((int)CloudServiceOptions.StorageStandard, (int)CloudServiceLocations.AustraliaEast);

            var encryptedDatabaseConnectionString = _encryptionService.Encrypt(expectedDatabaseConnectionString);
            var encryptedStorageConnectionString = _encryptionService.Encrypt(expectedStorageConnectionString);

            result.Name.ShouldBe(newTenant.Name);

            var tenantRecord = await _tenantRepository.GetAsync(result.Id);
            tenantRecord.CreationTime.ShouldBeGreaterThanOrEqualTo(startTime);
            tenantRecord.ConnectionStrings.ShouldNotBeNull();
            tenantRecord.ConnectionStrings.Count.ShouldBeGreaterThan(0);
            tenantRecord.ConnectionStrings[0].Value.ShouldBe(encryptedDatabaseConnectionString);

            var tenantResourceDatabase = await _tenantResourceRepository.FindAsync(t => t.TenantId == result.Id && t.CloudServiceOptionId == (int)CloudServiceOptions.DatabaseBasic, false);
            tenantResourceDatabase.ConnectionString.ShouldNotBeNullOrEmpty();
            tenantResourceDatabase.ConnectionString.ShouldNotContain("Password=");
            tenantResourceDatabase.ConnectionString.ShouldBe(encryptedDatabaseConnectionString);

            var tenantResourceStorage = await _tenantResourceRepository.FindAsync(t => t.TenantId == result.Id && t.CloudServiceOptionId == (int)CloudServiceOptions.StorageStandard, false);
            tenantResourceStorage.ConnectionString.ShouldNotBeNullOrEmpty();
            tenantResourceStorage.ConnectionString.ShouldNotContain("AccountName=");
            tenantResourceStorage.ConnectionString.ShouldBe(encryptedStorageConnectionString);

            using (_currentTenant.Change(result.Id))
            {
                var user = _userRepository.GetAsync(e => e.TenantId == result.Id && e.UserName == "admin").GetAwaiter().GetResult();
                user.Email.ShouldBe(newTenant.AdminEmailAddress);
            }
        }

        [Fact(Skip = SkipReasonForManualRun)]
        public async Task CreateTenantMwp_FreeTrial_WithSpecificOptions_ShouldStillGetDefaultResources()
        {
            // arrange
            var startTime = DateTime.Now;

            var newTenant = new MwpSaasTenantCreateDto
            {
                LocationId = (int)CloudServiceLocations.AustraliaEast,
                DatabaseOptionId = (int)CloudServiceOptions.DatabaseStandard,
                StorageOptionId = (int)CloudServiceOptions.StoragePremium,
                AdminEmailAddress = "tenantadmin@mwp.com",
                AdminPassword = _defaultAdminPassword,
                EditionId = Guid.NewGuid(),
                Name = "My tenant"
            };

            // act
            var result = await WithUnitOfWorkAsync(() => _mwpTenantAppService.CreateAsync(newTenant));

            // assert
            var expectedDatabaseConnectionString = await GetSharedResourceKey((int)CloudServiceOptions.DatabaseBasic, (int)CloudServiceLocations.AustraliaEast);
            var expectedStorageConnectionString = await GetSharedResourceKey((int)CloudServiceOptions.StorageStandard, (int)CloudServiceLocations.AustraliaEast);

            var encryptedDatabaseConnectionString = _encryptionService.Encrypt(expectedDatabaseConnectionString);
            var encryptedStorageConnectionString = _encryptionService.Encrypt(expectedStorageConnectionString);

            result.Name.ShouldBe(newTenant.Name);

            var tenantRecord = await _tenantRepository.GetAsync(result.Id);
            tenantRecord.CreationTime.ShouldBeGreaterThanOrEqualTo(startTime);
            tenantRecord.ConnectionStrings.ShouldNotBeNull();
            tenantRecord.ConnectionStrings.Count.ShouldBeGreaterThan(0);
            tenantRecord.ConnectionStrings[0].Value.ShouldBe(encryptedDatabaseConnectionString);

            var tenantResourceDatabase = await _tenantResourceRepository.FindAsync(t => t.TenantId == result.Id && t.CloudServiceOptionId == (int)CloudServiceOptions.DatabaseBasic);
            tenantResourceDatabase.ConnectionString.ShouldNotBeNullOrEmpty();
            tenantResourceDatabase.ConnectionString.ShouldNotContain("Password=");
            tenantResourceDatabase.ConnectionString.ShouldBe(encryptedDatabaseConnectionString);

            var tenantResourceStorage = await _tenantResourceRepository.FindAsync(t => t.TenantId == result.Id && t.CloudServiceOptionId == (int)CloudServiceOptions.StorageStandard);
            tenantResourceStorage.ConnectionString.ShouldNotBeNullOrEmpty();
            tenantResourceStorage.ConnectionString.ShouldNotContain("AccountName=");
            tenantResourceStorage.ConnectionString.ShouldBe(encryptedStorageConnectionString);

            using (_currentTenant.Change(result.Id))
            {
                var user = _userRepository.GetAsync(e => e.TenantId == result.Id && e.UserName == "admin").GetAwaiter().GetResult();
                user.Email.ShouldBe(newTenant.AdminEmailAddress);
            }
        }

        private async Task<string> GetSharedResourceKey(int optionId, int locationId)
        {
            var resource = await _sharedResourceRepository.GetSharedResourceByOptionId(optionId, locationId, true);
            var secret = _json.Deserialize<SharedResourceSecret>(_config[resource.SecretName]);
            return secret.Key;
        }
    }
}