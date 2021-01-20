using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Mwp.AzureStorage;
using Mwp.AzureStorage.File;
using Mwp.Form;
using Volo.Abp.EventBus.Local;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Saas.Tenants;

namespace Mwp.File
{
    public class AzureFileCleanupStorageClient : AzureFileStorageClient, IFileCleanupStorageClient
    {
        public AzureFileCleanupStorageClient(
            IConfiguration configRoot,
            CurrentTenant currentTenant,
            ITenantRepository tenantRepository,
            ITenantStorageConnectionProvider tenantStorageConnectionProvider,
            ILocalEventBus eventBus,
            IStringEncryptionService encryptionService) : base(configRoot, currentTenant, tenantRepository, tenantStorageConnectionProvider, eventBus, encryptionService)
        {
        }

        private bool IsMwpTables(string tableName)
        {
            return tableName.StartsWith(StorageTables.FormHistory)
                   && tableName.Length > StorageTables.FormHistory.Length
                   || tableName.StartsWith(StorageTables.UploadFileIndex)
                   && tableName.Length > StorageTables.UploadFileIndex.Length
                   || tableName.StartsWith(StorageTables.SubmissionHistory)
                   && tableName.Length > StorageTables.SubmissionHistory.Length;
        }

        public async Task DeleteTableStorageInAccount(Guid? tenantId = null)
        {
            var storageConnectionString = GetAzureStorageConnectionString();

            // Retrieve storage account information from connection string.
            var storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

            // Create a table client for interacting with the table service
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var tables = tableClient.ListTables().ToList();
            foreach (var table in tables)
            {
                if (IsMwpTables(table.Name))
                {
                    if (tenantId == null || table.Name.Contains(tenantId.ToString()))
                    {
                        await table.DeleteIfExistsAsync();
                    }
                }
            }
        }

        public async Task DeleteFileContainersInAccount(Guid? tenantId = null)
        {
            var blobServiceClient = new BlobServiceClient(GetAzureStorageConnectionString());
            var containers = blobServiceClient.GetBlobContainers();
            var items = containers.ToList();
            foreach (var item in items)
            {
                if (item.Name.StartsWith(StorageBlobRefs.UploadFile)
                    && item.Name.Length > StorageBlobRefs.UploadFile.Length)
                {
                    if (tenantId == null || item.Name.Contains(tenantId.ToString()))
                    {
                        await blobServiceClient.DeleteBlobContainerAsync(item.Name);
                    }
                }
            }
        }
    }
}