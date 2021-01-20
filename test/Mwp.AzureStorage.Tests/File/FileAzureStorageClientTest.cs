using System.Text;
using System.Threading.Tasks;
using Mwp.File;
using Shouldly;
using Xunit;

namespace Mwp.AzureStorage.File
{
    public sealed class FileAzureStorageClientTest : MwpAzureStorageTestBase
    {
        private readonly IFileStorageClient _client;

        public FileAzureStorageClientTest()
        {
            _client = GetRequiredService<IFileStorageClient>();
        }

        [Fact]
        public async Task UploadFileToStorage_WhenCalled_ShouldUploadFileSuccessfully()
        {
            var data = Encoding.UTF8.GetBytes("TEST DATA UPLOAD");
            var uploadResult = await _client.UploadFileToStorage(data, "text/plain", "test.txt", data.Length);
            uploadResult.ShouldNotBeNull();
            uploadResult.FileId.ShouldNotBeNullOrEmpty();

            var dlResult = await _client.DownloadFileFromStorage(uploadResult.FileId);
            dlResult.ContentType.ShouldBe("text/plain");
            dlResult.FileName.ShouldBe("test.txt");
            var dlText = Encoding.UTF8.GetString(dlResult.Data);
            dlText.ShouldBe("TEST DATA UPLOAD");
        }

        [Fact]
        public async Task DeleteFile_WhenCalled_ShouldUDeleteFileSuccessfully()
        {
            var data = Encoding.UTF8.GetBytes("TEST DATA UPLOAD 1234");
            var uploadResult = await _client.UploadFileToStorage(data, "text/plain", "test.txt", data.Length);

            var fileInfo = await _client.GetFileInfoFromStorage(uploadResult.FileId);
            fileInfo.ShouldNotBeNull();

            await _client.DeleteFile(uploadResult.FileId);

            fileInfo = await _client.GetFileInfoFromStorage(uploadResult.FileId);
            fileInfo.ShouldBeNull();
        }
    }
}