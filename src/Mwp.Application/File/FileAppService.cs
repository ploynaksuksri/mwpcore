using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.File.Events;
using Mwp.Permissions;
using Newtonsoft.Json.Linq;
using Volo.Abp;
using Volo.Abp.EventBus.Local;

namespace Mwp.File
{
    [RemoteService(false)]
    [Authorize(MwpPermissions.File.Default)]
    public class FileAppService : MwpAppService, IFileAppService
    {
        private readonly ILocalEventBus _eventBus;
        private readonly IFileStorageClient _fileStorageClient;

        public FileAppService(IFileStorageClient fileStorageClient, ILocalEventBus eventBus)
        {
            _fileStorageClient = fileStorageClient;
            _eventBus = eventBus;
        }

        public async Task<UploadFileResult> UploadFile(byte[] data, string contentType, string fileName, long fileLength)
        {
            var result = await _fileStorageClient.UploadFileToStorage(data, contentType, fileName, fileLength);

            await _eventBus.PublishAsync(new UploadedFileEventData(result));

            return result;
        }

        public Task<JObject> GetFileInfo(string fileId)
        {
            return _fileStorageClient.GetFileInfoFromStorage(fileId);
        }

        public Task<UploadFile> DownloadFile(string fileId)
        {
            return _fileStorageClient.DownloadFileFromStorage(fileId);
        }

        public async Task DeleteFile(string fileId)
        {
            await _fileStorageClient.DeleteFile(fileId);

            await _eventBus.PublishAsync(new DeletedFileEventData(fileId));
        }

        public async Task ClearAllUnusedFileInAllTenant()
        {
            await _fileStorageClient.ClearUnusedFileInAllTenants(DateTime.Now);
        }
    }
}