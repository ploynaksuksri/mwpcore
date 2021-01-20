using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Mwp.File
{
    public interface IFileStorageClient
    {
        Task<UploadFileResult> UploadFileToStorage(
            byte[] data,
            string contentType,
            string fileName,
            long length);

        Task<UploadFile> DownloadFileFromStorage(string fileId);

        Task<JObject> GetFileInfoFromStorage(string fileId);

        Task DeleteFile(string fileId);
        Task DeleteUnusedFiles(string[] fileHashes);
        Task<string[]> GetFilesHashByTimestampDate(DateTime date);
        Task ClearUnusedFileIndex(DateTime date);
        Task ClearUnusedFile(DateTime date);
        Task ClearUnusedFileInAllTenants(DateTime date);

        Task<string[]> GetFileIdsByReferredBy(string referredBy);
        Task UpdateFilesReferredBy(string[] fileIds, Guid? referredBy, FileReferrerType refererType);
        Task<int> CountFiles();
        Task<UploadFileResult> ReplaceFile(Guid fileId, byte[] fileContent, string newFilename = null, string contentType = null);
        Task<UploadFileResult> CopyFileById(string fileId);
    }
}