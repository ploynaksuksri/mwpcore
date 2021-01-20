using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Volo.Abp.Application.Services;

namespace Mwp.File
{
    public interface IFileAppService : IApplicationService
    {
        Task<UploadFileResult> UploadFile(byte[] data, string contentType, string fileName, long fileLength);

        Task<JObject> GetFileInfo(string fileId);

        Task<UploadFile> DownloadFile(string fileId);

        Task DeleteFile(string fileId);
    }
}