using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Sql.Fluent;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Extensions.Logging;
using Mwp.CloudService;
using Mwp.Provision;
using Mwp.SqlDatabase;
using Mwp.Tenants;
using Mwp.Tenants.Events.Request;
using Mwp.Tenants.Events.Result;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using IStorageManager = Mwp.Storage.IStorageManager;

namespace Mwp
{
    [ExposeServices(typeof(IResourceProvisionRequestManager))]
    public class ResourceSDKProvisionRequestManager : IResourceProvisionRequestManager
    {
        protected IDatabaseManager AzureDatabaseRM;
        protected IStorageManager AzureStorageRM;
        protected LocalDistributedEventBus LocalEventBus;
        protected ILogger Logger;

        public ResourceSDKProvisionRequestManager(
            IDatabaseManager azureDatabaseRm,
            IStorageManager azureStorageRM,
            LocalDistributedEventBus localEventBus,
            ILogger<ResourceSDKProvisionRequestManager> logger)
        {
            AzureDatabaseRM = azureDatabaseRm;
            AzureStorageRM = azureStorageRM;
            LocalEventBus = localEventBus;
            Logger = logger;
        }

        public async Task ProvisionDatabase(TenantResource resource)
        {
            var provisionResult = new DatabaseProvisionResultEventData
            {
                TenantId = resource.TenantId,
                ResourceId = resource.Id,
                SubscriptionId = resource.SubscriptionId,
                ResourceGroupName = resource.ResourceGroup,
                ServerName = resource.ServerName,
                Name = resource.Name,
                StatusId = ProvisionStatus.Initial,
                AdminEmailAddress = resource.AdminEmailAddress,
                AdminPassword = resource.AdminPassword
            };

            try
            {
                var isPremiumDatabase = (CloudServiceOptions)resource.CloudServiceOptionId == CloudServiceOptions.DatabasePremium;
                await (isPremiumDatabase ? CreatePremiumDatabase(resource) : CreateStandardDatabase(resource));
                provisionResult.StatusId = ProvisionStatus.Success;
            }
            catch (Exception exception)
            {
                Logger.LogError($"Error during database provision: {exception.Message}");
                provisionResult.StatusId = ProvisionStatus.Fail;
            }

            await LocalEventBus.PublishAsync(provisionResult);
        }

        public async Task ProvisionStorage(TenantResource resource)
        {
            var provisionResult = new StorageProvisionResultEventData
            {
                TenantId = resource.TenantId,
                ResourceId = resource.Id,
                SubscriptionId = resource.SubscriptionId,
                ResourceGroupName = resource.ResourceGroup,
                ServerName = resource.ServerName,
                Name = resource.Name,
                StatusId = ProvisionStatus.Initial
            };

            try
            {
                await CreatePremiumStorage(resource);
                provisionResult.ConnectionString = await GetStorageConnectionString(resource);
                provisionResult.StatusId = ProvisionStatus.Success;
            }
            catch (Exception exception)
            {
                Logger.LogError($"Error during storage provision: {exception.Message}");
                provisionResult.StatusId = ProvisionStatus.Fail;
            }

            await LocalEventBus.PublishAsync(provisionResult);
        }

        #region private methods

        private async Task<ISqlDatabase> CreatePremiumDatabase(TenantResource resource)
        {
            if (!await AzureDatabaseRM.CheckServerNameAvailabilityAsync(resource.ServerName))
            {
                throw new Exception($"Duplicate database server name: {resource.Name}");
            }

            var sqlServer = await AzureDatabaseRM.CreateSqlServer(resource.ResourceGroup, resource.ServerName, (CloudServiceLocations)resource.CloudServiceLocationId);
            return await AzureDatabaseRM.CreateDatabase(sqlServer, resource.Name);
        }

        private async Task<ISqlDatabase> CreateStandardDatabase(TenantResource resource)
        {
            await AzureDatabaseRM.CreateDatabase(resource.ResourceGroup, resource.ServerName, resource.Name);
            return await AzureDatabaseRM.AddDatabaseToPool(resource.ResourceGroup, resource.ServerName, resource.ElasticPoolName, resource.Name);
        }

        private async Task<IStorageAccount> CreatePremiumStorage(TenantResource resource)
        {
            var isNameAvailable = await AzureStorageRM.CheckAccountNameAvailability(resource.Name);
            if (!isNameAvailable)
            {
                throw new Exception($"Duplicate storage account name: {resource.Name}");
            }

            return await AzureStorageRM.CreateAsync(resource.ResourceGroup, resource.Name, (CloudServiceLocations)resource.CloudServiceLocationId);
        }

        private async Task<string> GetStorageConnectionString(TenantResource resource)
        {
            return await AzureStorageRM.GetConnectionString(resource.ResourceGroup, resource.Name);
        }

        #endregion private methods
    }
}