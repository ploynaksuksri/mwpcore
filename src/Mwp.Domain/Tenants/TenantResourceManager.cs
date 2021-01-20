using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mwp.CloudService;
using Mwp.Data;
using Mwp.Provision;
using Mwp.Tenants.Events.Request;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Json;
using Volo.Abp.Security.Encryption;
using Volo.Abp.Uow;
using Volo.Saas.Tenants;

namespace Mwp.Tenants
{
    public class TenantResourceManager : DomainService, ITenantResourceManager
    {
        protected readonly IResourceProvisionRequestManager ProvisionRequestManager;
        protected readonly IRepository<TenantResource> TenantResourcesRepo;
        protected readonly ISharedResourceRepository SharedResourceRepository;
        protected readonly ICloudServiceOptionRepository CloudServiceOptionRepository;
        protected readonly ITenantRepository TenantRepository;
        protected readonly ITenantExRepository TenantExRepository;
        protected readonly MwpTenantMigrationService TenantMigrationService;
        protected readonly IConfiguration Configuration;
        protected readonly IStringEncryptionService EncryptionService;
        protected readonly IJsonSerializer Json;

        public TenantResourceManager(
            IResourceProvisionRequestManager provisionRequestManager,
            IRepository<TenantResource> tenantResourcesRepo,
            ICloudServiceOptionRepository cloudServiceOptionRepository,
            ISharedResourceRepository sharedResourceRepository,
            ITenantRepository tenantRepository,
            ITenantExRepository tenantExRepository,
            MwpTenantMigrationService tenantMigrationService,
            IConfiguration configuration,
            IStringEncryptionService encryptionService,
            IJsonSerializer json)

        {
            ProvisionRequestManager = provisionRequestManager;
            TenantResourcesRepo = tenantResourcesRepo;
            CloudServiceOptionRepository = cloudServiceOptionRepository;
            SharedResourceRepository = sharedResourceRepository;
            TenantRepository = tenantRepository;
            TenantExRepository = tenantExRepository;
            TenantMigrationService = tenantMigrationService;
            Configuration = configuration;
            EncryptionService = encryptionService;
            Json = json;
        }

        public async Task ProvideTenantResources(TenantResourceRequest request)
        {
            Check.NotNull(request, nameof(request));

            await ProvideTenantDatabase(request);
            await ProvideTenantStorage(request);
        }

        public async Task<TenantResource> ProvideTenantDatabase(TenantResourceRequest request)
        {
            var resource = await CreateTenantResourceAsync(request, ProvisionResourceType.Database);
            resource = await TenantResourcesRepo.InsertAsync(resource, true);

            if (resource.IsProvisionRequired)
            {
                await ProvisionRequestManager.ProvisionDatabase(resource);
            }
            else
            {
                var tenant = await UpdateTenantConnectionString(resource.TenantId, resource.ConnectionString);
                await InitialiseTenantDatabase(tenant, request.AdminEmailAddress, request.AdminPassword);
                await SetTenantIsActive(request.TenantId, true);
            }

            return resource;
        }

        public async Task<TenantResource> ProvideTenantStorage(TenantResourceRequest request)
        {
            var resource = await CreateTenantResourceAsync(request, ProvisionResourceType.Storage);
            resource = await TenantResourcesRepo.InsertAsync(resource, true);

            if (resource.IsProvisionRequired)
            {
                await ProvisionRequestManager.ProvisionStorage(resource);
            }

            return resource;
        }

        [UnitOfWork]
        public async Task<Tenant> UpdateTenantConnectionString(Guid tenantId, string connectionString)
        {
            Logger.LogInformation($"Database connection string for tenant {tenantId} is {connectionString}");
            var tenant = await TenantRepository.GetAsync(tenantId);
            tenant.SetDefaultConnectionString(connectionString);
            return await TenantRepository.UpdateAsync(tenant, true);
        }

        [UnitOfWork]
        public async Task InitialiseTenantDatabase(Tenant tenant, string adminEmailAddress, string adminPassword)
        {
            await TenantMigrationService.InitialiseTenantDatabaseAsync(tenant, adminEmailAddress, adminPassword);
        }

        public async Task SetTenantIsActive(Guid tenantId, bool isActive)
        {
            var tenantEx = await TenantExRepository.GetByTenantIdAsync(tenantId);
            tenantEx.IsActive = isActive;
        }

        #region private methods

        private async Task<TenantResource> CreateTenantResourceAsync(TenantResourceRequest request, ProvisionResourceType type)
        {
            var cloudServiceOptionId = GetOptionByResourceType(request, type);
            var sharedResource = await SharedResourceRepository.GetSharedResourceByOptionId(cloudServiceOptionId, request.LocationId, request.IsFreeTrial);
            var resourceSecret = Json.Deserialize<SharedResourceSecret>(Configuration[sharedResource.SecretName]);

            var resource = new TenantResource(request.TenantId, request.TenantExId)
            {
                IsActive = false,
                CloudServiceLocationId = request.LocationId,
                CloudServiceOptionId = cloudServiceOptionId,
                ProvisionStatus = ProvisionStatus.Initial,
                ResourceGroup = resourceSecret.ResourceGroup,
                SubscriptionId = resourceSecret.SubscriptionId,
                IsProvisionRequired = sharedResource.CloudServiceOption.IsProvisionRequired,
                AdminEmailAddress = request.AdminEmailAddress,
                AdminPassword = request.AdminPassword
            };

            if (resource.IsProvisionRequired)
            {
                if (type == ProvisionResourceType.Database)
                {
                    var isPremiumDatabase = resource.CloudServiceOptionId == (int)CloudServiceOptions.DatabasePremium;
                    resource.Name = TransformDatabaseName(request.TenantName);
                    resource.ElasticPoolName = resourceSecret.Name;
                    resource.ServerName = isPremiumDatabase ? TransformDatabaseName(request.TenantName) : resourceSecret.Key;
                }

                if (type == ProvisionResourceType.Storage)
                {
                    resource.Name = TransformStorageName(request.TenantName);
                }
            }
            else
            {
                resource.Name = resourceSecret.Name;
                resource.ConnectionString = EncryptionService.Encrypt(resourceSecret.Key);
                resource.ProvisionStatus = ProvisionStatus.Success;
                resource.IsActive = true;
            }

            return resource;
        }

        private int GetOptionByResourceType(TenantResourceRequest request, ProvisionResourceType type)
        {
            var optionId = 0;
            switch (type)
            {
                case ProvisionResourceType.Database:
                    optionId = request.DatabaseOptionId;
                    break;

                case ProvisionResourceType.Storage:
                    optionId = request.StorageOptionId;
                    break;
            }

            return optionId;
        }

        private string TransformStorageName(string name)
        {
            var transformedName = TenantResourceConsts.ResourceNamePrefix + RemoveSpecialCharacters(name, true);
            if (transformedName.Length > TenantResourceConsts.MaxStorageNameLength)
            {
                return transformedName.Substring(0, TenantResourceConsts.MaxStorageNameLength);
            }

            return transformedName;
        }

        private string TransformDatabaseName(string name)
        {
            return TenantResourceConsts.ResourceNamePrefix + RemoveSpecialCharacters(name, true);
        }

        private string RemoveSpecialCharacters(string text, bool toLower = false)
        {
            text = text.Replace(" ", "");
            if (toLower)
            {
                return Regex.Replace(text.ToLower(), @"[^a-z0-9]+", "");
            }

            return Regex.Replace(text, @"[^a-zA-Z0-9]+", "");
        }

        #endregion private methods
    }
}