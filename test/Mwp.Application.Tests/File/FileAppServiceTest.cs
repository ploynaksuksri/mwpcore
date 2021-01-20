using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Mwp.CloudService;
using Mwp.Form;
using Mwp.Tenants;
using Mwp.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Saas.Tenants;
using Xunit;

namespace Mwp.File
{
    public class FileAppServiceTest : MwpApplicationTestBase
    {

        private readonly ICurrentTenant _currentTeant;
        private readonly IFileStorageClient _fileStorage;
        private readonly IFormAppService _formAppService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IRepository<TenantResource> _tenantResourceRepo;

        private readonly ITenantStorageConnectionProvider _tenantStorageConnStrProvider;

        private readonly IFileCleanupStorageClient _cleanupStorageClient;

        public FileAppServiceTest()
        {
            _tenantStorageConnStrProvider = GetRequiredService<ITenantStorageConnectionProvider>();
            _tenantRepository = GetRequiredService<ITenantRepository>();
            _formAppService = GetRequiredService<IFormAppService>();
            _tenantResourceRepo = GetRequiredService<IRepository<TenantResource>>();
            _fileStorage = GetRequiredService<IFileStorageClient>();
            _currentTeant = GetRequiredService<ICurrentTenant>();
            _cleanupStorageClient = GetRequiredService<IFileCleanupStorageClient>();
        }

        [Fact]
        public async Task DeleteTableStorageInAccount()
        {
            await _cleanupStorageClient.DeleteTableStorageInAccount();
        }

        [Fact]
        public async Task DeleteFileContainersInAccount()
        {
            await _cleanupStorageClient.DeleteFileContainersInAccount();
        }

        [Fact]
        public async Task ClearUnusedFileInAllTenants()
        {
            var tenant1Id = (await _tenantRepository.FindByNameAsync("T1")).Id;
            var tenant2Id = (await _tenantRepository.FindByNameAsync("T2")).Id;

            await EnsureTenantHaveDefaultStorageConnectionString(tenant1Id);
            await EnsureTenantHaveDefaultStorageConnectionString(tenant2Id);

            _tenantStorageConnStrProvider.RebuildTenantConnectionStringMap();
            await CreateSubmissionWithFileIds(tenant1Id);
            await CreateSubmissionWithFileIds(tenant2Id);
            await _fileStorage.ClearUnusedFileInAllTenants(DateTime.Now);

            var tenant1FileCount = await CountFileExistInTenantTableStorage(tenant1Id);
            tenant1FileCount.ShouldBe(2);
            var tenant2FileCount = await CountFileExistInTenantTableStorage(tenant2Id);
            tenant2FileCount.ShouldBe(2);

            await _cleanupStorageClient.DeleteFileContainersInAccount(tenant1Id);
            await _cleanupStorageClient.DeleteFileContainersInAccount(tenant2Id);
            await _cleanupStorageClient.DeleteTableStorageInAccount(tenant1Id);
            await _cleanupStorageClient.DeleteTableStorageInAccount(tenant2Id);
        }

        [Fact]
        public async Task ClearUnusedFile_Should_RemoveFileThatNoLongerNeeded()
        {
            var fileIds = await UploadFileForTest(10);
            var fileInSubmission = fileIds.Where(x => fileIds.IndexOf(x) % 2 == 0).ToArray();
            await CreateNewSubmission(fileInSubmission);
            await _fileStorage.ClearUnusedFile(DateTime.Now);
        }

        [Fact]
        public void GetAllFileIdsInForm()
        {
            var formJson = MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(
                typeof(FormAppServiceTest).Assembly,
                "Mwp.Form.json.form-with-documents.json");
            var formJObj = JObject.Parse(formJson);
            var fileIds = _formAppService.GetAllFileIdsInForm(formJObj);
            fileIds.ShouldNotBeNull();
            fileIds.Length.ShouldBeGreaterThan(0);
        }

        private async Task<int> CountFileExistInTenantTableStorage(Guid tenantId)
        {
            using (_currentTeant.Change(tenantId))
            {
                return await _fileStorage.CountFiles();
            }
        }

        private async Task EnsureTenantHaveDefaultStorageConnectionString(Guid tenantId)
        {
            using (_currentTeant.Change(null))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var basedTenantStorageRes = _tenantResourceRepo
                        .FirstOrDefault(tr => tr.TenantId == tenantId && tr.CloudServiceOption.CloudService.CloudServiceTypeId == (int)CloudServiceTypes.Storage);

                    if (basedTenantStorageRes != null)
                    {
                        basedTenantStorageRes.ConnectionString = null;
                        await _tenantResourceRepo.UpdateAsync(basedTenantStorageRes);
                    }
                });
            }
        }

        private async Task CreateSubmissionWithFileIds(Guid tenantId)
        {
            using (_currentTeant.Change(tenantId))
            {
                var fileIds = await UploadFileForTest(4);
                var fileInSubmission = fileIds.Where(x => fileIds.IndexOf(x) % 2 == 0).ToArray();
                await CreateNewSubmission(fileInSubmission, tenantId);
            }
        }

        private async Task<string[]> UploadFileForTest(int count)
        {
            var allFileIds = new List<string>();
            for (var i = 0; i < count; i++)
            {
                var uuid = Guid.NewGuid();
                var data = Encoding.UTF8.GetBytes($"TEST_FILE_TO_CLEAR_UNUSED_INDEX_{i}_{uuid}");
                var uploadResult = await _fileStorage.UploadFileToStorage(data, "text/plain",
                    $"test_file_to_clear_unused_index__{i}_{uuid}.txt", data.Length);
                allFileIds.Add(uploadResult.FileId);
            }

            return allFileIds.ToArray();
        }

        private async Task<string> CreateNewForm(Guid? tenantId = null)
        {
            var token = new JObject();
            token["name"] = tenantId == null ? "TestForm" : $"TestForm-{tenantId}";
            var components = new JArray();
            components.Add(JToken.FromObject(new
            {
                key = "files",
                label = "Files",
                type = FormAppService.FileUploadComponentNames[0]
            }));
            token["components"] = components;
            var json = await WithUnitOfWorkAsync(async () => await _formAppService.SaveForm(token));
            return json;
        }

        private async Task CreateNewSubmission(string[] fileIds, Guid? tenantId = null)
        {
            var saveFormJson = await CreateNewForm(tenantId);
            var saveFormJObj = JObject.Parse(saveFormJson);
            var formId = new Guid(saveFormJObj["_id"]!.Value<string>());
            var obj = new
            {
                data = new
                {
                    files = fileIds.Select(x => new { fileId = x }),
                    submit = true
                }
            };
            var jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            var saveResult = await WithUnitOfWorkAsync(async () => await _formAppService.SaveSubmission(formId, jObj));
            var resultToken = JObject.Parse(saveResult);
            resultToken.ShouldNotBeNull();
            resultToken["_id"]!.Value<string>().ShouldNotBeNull();
        }
    }
}