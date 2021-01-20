using Microsoft.Azure.Cosmos.Table;

namespace Mwp.AzureStorage.TableEntities
{
    public class UploadFileTableEntity : TableEntity
    {
        public string FileHash { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public string ReferredBy { get; set; }
        public string ReferrerType { get; set; }
    }
}