using Microsoft.AspNetCore.StaticFiles;

namespace Mwp.File
{
    public static class FileUtil
    {
        public static string GetMimeMapping(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        public static string GetFileExtension(string fileName)
        {
            return fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
        }

        public static string GetFileNameWithoutExtension(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf('.'));
        }
    }
}