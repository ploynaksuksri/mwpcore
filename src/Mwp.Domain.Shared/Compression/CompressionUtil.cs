using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Mwp.Compression
{
    public static class CompressionUtil
    {
        public static byte[] CompressString(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream inStream = null;
            MemoryStream outStream = null;
            try
            {
                inStream = new MemoryStream();
                using (var zip = new GZipStream(inStream, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                }

                inStream.Position = 0;
                outStream = new MemoryStream();

                var compressed = new byte[inStream.Length];
                inStream.Read(compressed, 0, compressed.Length);

                var gzBuffer = new byte[compressed.Length + 4];
                Buffer.BlockCopy(
                    compressed,
                    0,
                    gzBuffer,
                    4,
                    compressed.Length);
                Buffer.BlockCopy(
                    BitConverter.GetBytes(buffer.Length),
                    0,
                    gzBuffer,
                    0,
                    4);
                return gzBuffer;
            }
            finally
            {
                inStream?.Dispose();
                outStream?.Dispose();
            }
        }

        public static string DecompressToString(byte[] input)
        {
            using (var source = new MemoryStream(input))
            {
                var lengthBytes = new byte[4];
                source.Read(lengthBytes, 0, 4);

                var length = BitConverter.ToInt32(lengthBytes, 0);
                using (var decompressionStream = new GZipStream(source,
                    CompressionMode.Decompress))
                {
                    var data = new byte[length];
                    decompressionStream.Read(data, 0, length);
                    return Encoding.UTF8.GetString(data);
                }
            }
        }
    }
}