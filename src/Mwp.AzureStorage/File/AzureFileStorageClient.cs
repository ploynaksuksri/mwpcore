using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Protocol;
using Microsoft.Extensions.Configuration;
using Mwp.AzureStorage.TableEntities;
using Mwp.File;
using Mwp.File.Events;
using Mwp.Form;
using Newtonsoft.Json.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Saas.Tenants;

namespace Mwp.AzureStorage.File
{
    public class AzureFileStorageClient : AzureStorageClientBase, IFileStorageClient, ITransientDependency
    {
        const string NONE_REFERRER = "NONE";

        public AzureFileStorageClient(
            IConfiguration configRoot,
            CurrentTenant currentTenant,
            ITenantRepository tenantRepository,
            ITenantStorageConnectionProvider tenantStorageConnectionProvider,
            ILocalEventBus eventBus,
            IStringEncryptionService encryptionService)
            : base(configRoot, currentTenant, tenantRepository, tenantStorageConnectionProvider, eventBus, encryptionService)
        {
        }

        private async Task<UploadFileResult> UploadFileToStorage(
            CloudTable tableClient,
            BlobContainerClient blobContainer,
            byte[] data,
            string contentType,
            string fileName,
            string previousFileId = null)
        {
            var hasher = SHA256.Create();
            var fileHash = hasher.ComputeHash(data);
            var hashStr = BitConverter.ToString(fileHash).Replace("-", "");
            var entity = new UploadFileTableEntity
            {
                PartitionKey = "-",
                RowKey = Guid.NewGuid().ToString(),
                FileHash = hashStr,
                FileName = fileName,
                ContentType = contentType,
                Length = data.LongLength,
                ReferredBy = string.IsNullOrEmpty(previousFileId) ? NONE_REFERRER : previousFileId
            };

            await InsertOrReplaceEntity(tableClient, entity);
            var first2LettersOfHash = hashStr.Substring(0, 2);
            var remainHash = hashStr.Substring(2);

            var blobRef = blobContainer.GetBlobClient($"{first2LettersOfHash}/{remainHash}");
            var isExist = await blobRef.ExistsAsync();
            if (!isExist)
            {
                using (var ms = new MemoryStream(data))
                {
                    await blobRef.UploadAsync(ms, overwrite: true); // to fix Azure Storage Emulator bug
                }
            }

            return new UploadFileResult
            {
                FileId = entity.RowKey,
                FileName = fileName,
                FileSize = (int)data.LongLength,
                FileHash = hashStr
            };
        }

        public async Task<UploadFileResult> UploadFileToStorage(
            byte[] data,
            string contentType,
            string fileName,
            long length)
        {
            var tableClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var container = await CreateBlobContainer(StorageBlobRefs.UploadFile);
            var result = await UploadFileToStorage(tableClient, container, data, contentType, fileName);
            return result;
        }

        public async Task<UploadFile> DownloadFileFromStorage(string fileId)
        {
            var tableClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var retrieveOperation = TableOperation.Retrieve<UploadFileTableEntity>("-", fileId);
            var result = await tableClient.ExecuteAsync(retrieveOperation);
            if (result == null || result.HttpStatusCode != 200)
            {
                return null;
            }

            var entity = result.Result as UploadFileTableEntity;
            var first2LettersOfHash = entity?.FileHash.Substring(0, 2);
            var remainHash = entity?.FileHash.Substring(2);
            var container = await CreateBlobContainer(StorageBlobRefs.UploadFile);
            var blobRef = container.GetBlobClient($"{first2LettersOfHash}/{remainHash}");
            var isExist = await blobRef.ExistsAsync();
            if (!isExist)
            {
                return null;
            }

            var dto = new UploadFile();
            await using var ms = new MemoryStream();
            var cancelToken = new CancellationToken();
            await blobRef.DownloadToAsync(ms, cancelToken);
            dto.Data = ms.ToArray();
            dto.FileName = entity?.FileName;
            dto.ContentType = entity?.ContentType;
            return dto;
        }

        public async Task<JObject> GetFileInfoFromStorage(string fileId)
        {
            var tableClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var entity = await GetFileInfo(tableClient, fileId);
            if (entity == null)
            {
                return null;
            }

            return JObject.FromObject(new
            {
                FileId = entity.RowKey,
                entity.FileName,
                entity.FileHash,
                entity.ContentType,
                entity.Length,
                entity.ReferredBy,
                entity.ReferrerType
            });
        }

        public async Task DeleteFile(string fileId)
        {
            var tableClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var retrieveOperation = TableOperation.Retrieve<UploadFileTableEntity>("-", fileId);
            var result = await tableClient.ExecuteAsync(retrieveOperation);
            if (result == null || result.HttpStatusCode != 200)
            {
                return;
            }

            var entity = result.Result as UploadFileTableEntity;
            var delOperation = TableOperation.Delete(entity);
            await tableClient.ExecuteAsync(delOperation);
            var container = await CreateBlobContainer(StorageBlobRefs.UploadFile);
            await DeleteFileInBlobIfNoMoreFileIndexWithHashExist(
                tableClient,
                container,
                entity?.FileHash);
        }

        public async Task DeleteUnusedFiles(string[] fileHashes)
        {
            var tableClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var container = await CreateBlobContainer(StorageBlobRefs.UploadFile);
            var uniqueHashes = fileHashes.Distinct();
            foreach (var fileHash in uniqueHashes)
            {
                await DeleteFileInBlobIfNoMoreFileIndexWithHashExist(
                    tableClient,
                    container,
                    fileHash);
            }
        }

        public async Task<string[]> GetFilesHashByTimestampDate(DateTime date)
        {
            var qry = BuildQueryByTimestamp(date);
            qry.Select(new[]
            {
                nameof(UploadFileTableEntity.RowKey),
                nameof(UploadFileTableEntity.PartitionKey),
                nameof(UploadFileTableEntity.FileHash)
            });
            var tableClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var files = await tableClient.ExecuteQuerySegmentedAsync(qry, null);
            return files.Results.Select(x => x.FileHash).ToArray();
        }

        public async Task ClearUnusedFileIndex(DateTime date)
        {
            var qry = BuildQueryByTimestamp(date);
            qry.Select(new[]
            {
                nameof(UploadFileTableEntity.RowKey),
                nameof(UploadFileTableEntity.PartitionKey),
                nameof(UploadFileTableEntity.ReferredBy)
            });
            var tableClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var files = await tableClient.ExecuteQuerySegmentedAsync(qry, null);
            var toRemoveFiles = files.Results.Where(x => x.ReferredBy == NONE_REFERRER).ToList();
            await DeleteFileIndexes(tableClient, toRemoveFiles);
        }

        public async Task ClearUnusedFile(DateTime date)
        {
            var fileHashes = await GetFilesHashByTimestampDate(date);
            await ClearUnusedFileIndex(date);
            await DeleteUnusedFiles(fileHashes);
        }

        public async Task ClearUnusedFileInAllTenants(DateTime date)
        {
            _tenantStorageConnStrProvider.RebuildTenantConnectionStringMap();
            var tenants = await _tenantRepository.GetListAsync();
            foreach (var tenant in tenants)
            {
                using (_currentTenant.Change(tenant.Id))
                {
                    await ClearUnusedFile(date);
                }
            }
        }

        public async Task<string[]> GetFileIdsByReferredBy(string referredBy)
        {
            var tblClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var qry = new TableQuery<UploadFileTableEntity>();
            qry.Where(
                TableQuery.GenerateFilterCondition(
                    nameof(UploadFileTableEntity.ReferredBy),
                    QueryComparisons.Equal,
                    referredBy));
            var result = await tblClient.ExecuteQuerySegmentedAsync(qry, null);
            return result == null ? new string[0] : result.ToList().Select(x => x.RowKey).ToArray();
        }

        public async Task UpdateFilesReferredBy(string[] fileIds, Guid? referredBy, FileReferrerType referrerType)
        {
            var tblClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var chunks = GetOptimalChunksForAzureStorage(fileIds);
            foreach (var chunk in chunks)
            {
                var batch = new TableBatchOperation();
                foreach (var fileId in chunk)
                {
                    var entity = new DynamicTableEntity
                    {
                        RowKey = fileId,
                        PartitionKey = "-"
                    };
                    var referredByStr = referredBy == null ? NONE_REFERRER : referredBy.GetValueOrDefault().ToString();
                    entity.Properties.Add(
                        nameof(UploadFileTableEntity.ReferredBy),
                        EntityProperty.GeneratePropertyForString(referredByStr));
                    entity.Properties.Add(nameof(UploadFileTableEntity.ReferrerType),
                        EntityProperty.GeneratePropertyForString(referrerType.ToString()));
                    batch.InsertOrMerge(entity);
                }

                await tblClient.ExecuteBatchAsync(batch);
            }
        }

        public async Task<int> CountFiles()
        {
            var tblClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var qry = new TableQuery<UploadFileTableEntity>();
            qry.Select(new List<string> { "PartitionKey", "RowKey" });
            var result = await tblClient.ExecuteQuerySegmentedAsync(qry, null);
            return result.Results.Count;
        }


        public async Task<UploadFileResult> ReplaceFile(Guid fileId, byte[] fileContent, string newFileName = null, string contentType = null)
        {
            var tblClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var blobContainer = await CreateBlobContainer(StorageBlobRefs.UploadFile);
            var previousFile = await GetFileInfo(tblClient, fileId.ToString());
            return await UploadFileToStorage(
                tblClient,
                blobContainer,
                fileContent,
                previousFile != null ? previousFile.ContentType : contentType,
                string.IsNullOrEmpty(newFileName) ? previousFile?.FileName : newFileName,
                fileId.ToString());
        }

        public async Task<UploadFileResult> CopyFileById(string fileId)
        {
            var tblClient = await CreateTableAsync(StorageTables.UploadFileIndex);
            var originalFile = await GetFileInfo(tblClient, fileId);
            var newFileId = Guid.NewGuid().ToString();
            var entity = new UploadFileTableEntity
            {
                PartitionKey = "-",
                RowKey = newFileId,
                FileHash = originalFile.FileHash,
                FileName = originalFile.FileName,
                ContentType = originalFile.ContentType,
                Length = originalFile.Length,
                ReferredBy = NONE_REFERRER
            };
            await InsertOrReplaceEntity(tblClient, entity);
            return new UploadFileResult
            {
                FileId = newFileId,
                FileHash = originalFile.FileHash,
                FileName = originalFile.FileName,
                FileSize = (int)originalFile.Length
            };
        }

        private TableQuery<UploadFileTableEntity> BuildQueryByTimestamp(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddHours(23.90d);
            var qry = new TableQuery<UploadFileTableEntity>();
            var geStartDayQry = TableQuery.GenerateFilterConditionForDate(
                nameof(UploadFileTableEntity.Timestamp),
                QueryComparisons.GreaterThanOrEqual,
                startOfDay);
            var leEndDayQry = TableQuery.GenerateFilterConditionForDate(
                nameof(UploadFileTableEntity.Timestamp),
                QueryComparisons.LessThanOrEqual,
                endOfDay);
            qry.Where(
                TableQuery.CombineFilters(
                    geStartDayQry,
                    TableOperators.And,
                    leEndDayQry));

            return qry;
        }

        private async Task DeleteFileIndexes(CloudTable tableClient, IList<UploadFileTableEntity> files)
        {
            if (!files.Any())
            {
                return;
            }

            var chunks = GetOptimalChunksForAzureStorage(files);
            foreach (var chunk in chunks)
            {
                var batch = new TableBatchOperation();
                foreach (var file in chunk)
                {
                    var entity = new DynamicTableEntity
                    {
                        RowKey = file.RowKey,
                        PartitionKey = file.PartitionKey,
                        ETag = file.ETag
                    };
                    batch.Delete(entity);
                }

                await tableClient.ExecuteBatchAsync(batch);
            }

            await EventBus.PublishAsync(new DeletedFileIndexEventData(files.Select(f => f.RowKey).ToList()));
        }

        public static List<List<T>> SplitIntoChunks<T>(IList<T> list, int chunkSize)
        {
            var items = list.ToList();
            return items
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static List<List<T>> GetOptimalChunksForAzureStorage<T>(IList<T> list)
        {
            return SplitIntoChunks(list, TableConstants.TableServiceBatchMaximumOperations);
        }

        private async Task DeleteFileInBlobIfNoMoreFileIndexWithHashExist(
            CloudTable tbl,
            BlobContainerClient container,
            string hash)
        {
            if (tbl == null
                || container == null
                || string.IsNullOrEmpty(hash))
            {
                return;
            }

            var isFileIndexExist = await IsUploadFileWithHashExist(tbl, hash);
            if (isFileIndexExist)
            {
                return;
            }

            var first2LettersOfHash = hash.Substring(0, 2);
            var remainHash = hash.Substring(2);
            var blobRef = container.GetBlobClient($"{first2LettersOfHash}/{remainHash}");
            await blobRef.DeleteIfExistsAsync();
        }

        private async Task<bool> IsUploadFileWithHashExist(CloudTable tbl, string hash)
        {
            var qry = new TableQuery<UploadFileTableEntity>();
            qry.Where($"FileHash eq '{hash}'");
            qry.Select(new[]
            {
                nameof(UploadFileTableEntity.RowKey),
                nameof(UploadFileTableEntity.PartitionKey)
            });
            var result = await tbl.ExecuteQuerySegmentedAsync(qry, null);
            return result?.Results != null && result.Results.Count > 0;
        }

        private async Task<UploadFileTableEntity> GetFileInfo(CloudTable tableClient, string fileId)
        {
            var retrieveOperation = TableOperation.Retrieve<UploadFileTableEntity>("-", fileId);
            var result = await tableClient.ExecuteAsync(retrieveOperation);
            if (result == null || result.HttpStatusCode != 200)
            {
                return null;
            }

            var entity = result.Result as UploadFileTableEntity;
            return entity;
        }
    }
}