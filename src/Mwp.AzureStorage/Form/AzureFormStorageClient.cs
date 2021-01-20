using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Mwp.Compression;
using Mwp.Form;
using Newtonsoft.Json.Linq;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Saas.Tenants;

namespace Mwp.AzureStorage.Form
{
    public class AzureFormStorageClient : AzureStorageClientBase, IFormStorageClient, ITransientDependency
    {
        private readonly IBlobContainer<MwpFormBlobContainer> _formBlobContainer;

        public AzureFormStorageClient(
            IConfiguration configRoot,
            CurrentTenant currentTenant,
            ITenantRepository tenantRepository,
            ITenantStorageConnectionProvider tenantStorageConnectionProvider,
            IBlobContainer<MwpFormBlobContainer> formBlobContainer,
            ILocalEventBus eventBus,
            IStringEncryptionService encryptionService)
            : base(
                configRoot,
                currentTenant,
                tenantRepository,
                tenantStorageConnectionProvider,
                eventBus,
                encryptionService)
        {
            _formBlobContainer = formBlobContainer;
        }

        public async Task<string> SaveFormHistory(Mwp.Form.Form form)
        {
            var rowKey = form.CurrentVersion.ToString();
            var formObj = JObject.Parse(form.Data);
            var tblClient = await CreateTableAsync(StorageTables.FormHistory);
            var entity = new DynamicTableEntity(
                formObj["_id"]!.Value<string>(),
                rowKey);
            var compressedData = CompressionUtil.CompressString(formObj.ToString());
            entity.Properties.Add("data", EntityProperty.GeneratePropertyForByteArray(compressedData));
            await InsertOrReplaceDynamicEntity(tblClient, entity);
            return rowKey;
        }

        public async Task<JObject> GetFormHistory(Guid formId, string rowKey)
        {
            var tblClient = await CreateTableAsync(StorageTables.FormHistory);
            var retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(formId.ToString(), rowKey);
            var result = await tblClient.ExecuteAsync(retrieveOperation);
            var row = result.Result as DynamicTableEntity;
            if (row == null || !row.Properties.ContainsKey("data"))
            {
                return null;
            }

            var compressedData = row.Properties["data"].BinaryValue;
            if (compressedData == null)
            {
                return null;
            }

            var data = CompressionUtil.DecompressToString(compressedData);
            var formToken = JObject.Parse(data);
            formToken[FormIoProps.Timestamp] = row.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return formToken;
        }

        public async Task<string> SaveSubmissionHistory(Submission submission)
        {
            var submissionObj = JObject.Parse(submission.Data);
            var rowKey = submissionObj.Value<JObject>(FormIoProps.MwpMetaData)?.Value<string>(FormIoProps.CurrentVersion);
            var tblClient = await CreateTableAsync(StorageTables.SubmissionHistory);
            var entity = new DynamicTableEntity(
                submissionObj["_id"]!.Value<string>(),
                rowKey);
            var compressedData = CompressionUtil.CompressString(submissionObj.ToString());
            entity.Properties.Add("data", EntityProperty.GeneratePropertyForByteArray(compressedData));
            await InsertOrReplaceDynamicEntity(tblClient, entity);
            return rowKey;
        }

        public async Task<JObject> GetSubmissionHistory(Guid submissionId, string rowKey)
        {
            var tblClient = await CreateTableAsync(StorageTables.SubmissionHistory);
            var retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(submissionId.ToString(), rowKey);
            var result = await tblClient.ExecuteAsync(retrieveOperation);
            var row = result.Result as DynamicTableEntity;
            if (row == null || !row.Properties.ContainsKey("data"))
            {
                return null;
            }

            var compressedData = row.Properties["data"].BinaryValue;
            if (compressedData == null)
            {
                return null;
            }

            var data = CompressionUtil.DecompressToString(compressedData);
            return JObject.Parse(data);
        }

        public async Task<JArray> ListFormHistory(Guid formId)
        {
            var tblClient = await CreateTableAsync(StorageTables.FormHistory);
            var qry = new TableQuery();
            qry.Select(new List<string> { "PartitionKey", "RowKey", "Timestamp" });
            qry.Where($"PartitionKey eq '{formId}'");
            var result = await tblClient.ExecuteQuerySegmentedAsync(qry, null);
            var rows = result.Results.ToList()
                .OrderByDescending(x => x.Timestamp)
                .Select(x =>
                {
                    var row = new
                    {
                        x.PartitionKey,
                        x.RowKey,
                        Timestamp = x.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    };
                    return row;
                }).ToList();
            return JArray.FromObject(rows);
        }

        public async Task<JArray> ListSubmissionHistory(Guid submissionId)
        {
            var tblClient = await CreateTableAsync(StorageTables.SubmissionHistory);
            var qry = new TableQuery();
            qry.Select(new List<string> { "PartitionKey", "RowKey", "Timestamp" });
            qry.Where($"PartitionKey eq '{submissionId}'");
            var result = await tblClient.ExecuteQuerySegmentedAsync(qry, null);
            var rows = result.Results.ToList()
                .OrderByDescending(x => x.Timestamp)
                .Select(x =>
                {
                    var row = new
                    {
                        x.PartitionKey,
                        x.RowKey,
                        Timestamp = x.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    };
                    return row;
                }).ToList();
            return JArray.FromObject(rows);
        }

        public async Task<string> SaveMessageToFormBlobContainer(string message)
        {
            var hasher = SHA256.Create();
            var data = Encoding.UTF8.GetBytes(message);
            var fileHash = hasher.ComputeHash(data);
            var hashStr = BitConverter.ToString(fileHash).Replace("-", "");
            var isFileExist = await _formBlobContainer.ExistsAsync(hashStr);
            if (isFileExist)
            {
                return hashStr;
            }

            await _formBlobContainer.SaveAsync(hashStr, data);
            return hashStr;
        }

        public async Task<string> GetMessageFromFormBlobContainer(string hash)
        {
            var data = await _formBlobContainer.GetAllBytesAsync(hash);
            var message = Encoding.UTF8.GetString(data);
            return message;
        }
    }
}