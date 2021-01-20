using System.IO;
using System.Reflection;

namespace Mwp.Utilities
{
    public static class MwpEmbeddedResourceUtils
    {
        public static string ReadStringFromEmbededResource(Assembly assembly, string resourceName)
        {
            string result = null;
            Stream stream = null;
            try
            {
                stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        stream = null;
                        result = reader.ReadToEnd();
                    }
                }
            }
            finally
            {
                stream?.Dispose();
            }

            return result;
        }
    }
}