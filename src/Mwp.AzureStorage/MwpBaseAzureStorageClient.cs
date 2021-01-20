using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Mwp.Form;
using Volo.Abp.EventBus.Local;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Saas.Tenants;

namespace Mwp.AzureStorage
{
    public abstract class AzureStorageClientBase
    {
        protected readonly ICurrentTenant _currentTenant;
        protected readonly ITenantRepository _tenantRepository;
        protected readonly ITenantStorageConnectionProvider _tenantStorageConnStrProvider;
        protected readonly IConfiguration ConfigRoot;
        protected readonly ILocalEventBus EventBus;
        protected readonly IStringEncryptionService EncryptionService;

        protected AzureStorageClientBase(
            IConfiguration configRoot,
            ICurrentTenant currentTenant,
            ITenantRepository tenantRepository,
            ITenantStorageConnectionProvider tenantStorageConnStrProvider,
            ILocalEventBus eventBus,
            IStringEncryptionService encryptionService)

        {
            ConfigRoot = configRoot;
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
            _tenantStorageConnStrProvider = tenantStorageConnStrProvider;
            EventBus = eventBus;
            EncryptionService = encryptionService;
        }

        private string GetTenantIdAsString()
        {
            return _currentTenant?.Id?.ToString();
        }

        protected string GetTenantId(string overrideTenantId = null)
        {
            if (!string.IsNullOrEmpty(overrideTenantId))
            {
                return overrideTenantId;
            }

            return GetTenantIdAsString();
        }

        protected virtual string GetAzureStorageConnectionString(string tenantId = null)
        {
            var currentTenantId = _currentTenant?.Id;

            if (Guid.TryParse(tenantId, out var tryTenantId))
            {
                currentTenantId = tryTenantId;
            }

            if (currentTenantId != null)
            {
                var conStr = _tenantStorageConnStrProvider.GetTenantStorageConnectionString(currentTenantId.GetValueOrDefault());
                if (!string.IsNullOrEmpty(conStr))
                {
                    return conStr;
                }
            }

            return ConfigRoot["Storage:ConnectionString"];
        }

        protected virtual CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            return storageAccount;
        }

        protected virtual async Task<DynamicTableEntity> InsertOrReplaceDynamicEntity(
            CloudTable table,
            DynamicTableEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var insertOrMergeOperation = TableOperation.InsertOrReplace(entity);
            var result = await table.ExecuteAsync(insertOrMergeOperation);
            if (result.HttpStatusCode == 200)
            {
                return result.Result as DynamicTableEntity;
            }

            return null;
        }

        protected virtual string GetTenantTableName(string baseTableName, string tenantId)
        {
            return string.IsNullOrEmpty(tenantId) ? baseTableName : $"{baseTableName}{tenantId.Replace("-", "")}";
        }

        protected virtual async Task<CloudTable> CreateTableAsync(string baseTableName, string overrideTenantId = null)
        {
            var tenantId = GetTenantId(overrideTenantId);
            var tableName = GetTenantTableName(baseTableName, tenantId);
            var storageConnectionString = GetAzureStorageConnectionString(tenantId);

            // Retrieve storage account information from connection string.
            var storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

            // Create a table client for interacting with the table service
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            // Create a table client for interacting with the table service 
            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        protected virtual string GetTenantBlobContainerName(string baseContainerName, string tenantId)
        {
            return string.IsNullOrEmpty(tenantId)
                ? baseContainerName
                : $"{baseContainerName}{tenantId.Replace("-", "")}";
        }

        protected virtual async Task<BlobContainerClient> CreateBlobContainer(string baseContainerName,
            string tenantId = null)
        {
            var containerName = GetTenantBlobContainerName(baseContainerName, GetTenantId(tenantId));
            // Create a BlobServiceClient object which will be used to create a container client
            var blobServiceClient = new BlobServiceClient(GetAzureStorageConnectionString(tenantId));

            // Create the container and return a container client object
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            return containerClient;
        }

        protected virtual async Task<ITableEntity> InsertOrReplaceEntity(
            CloudTable table,
            ITableEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var insertOrMergeOperation = TableOperation.InsertOrReplace(entity);
            var result = await table.ExecuteAsync(insertOrMergeOperation);
            if (result.HttpStatusCode == 200)
            {
                return result.Result as ITableEntity;
            }

            return null;
        }
    }
}