using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySockets.Compression
{
    public class GZip : ICompression
    {
        /// <summary>
        /// Uses GZIP to compress the data.
        /// </summary>
        /// <param name="data">The byte array you want to compress.</param>
        /// <returns></returns>
        public byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var gz = new GZipStream(ms, CompressionMode.Compress))
                {
                    gz.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Uses GZIP to decompress the data.
        /// </summary>
        /// <param name="data">The byte array that you want to decompress.</param>
        /// <returns></returns>
        public byte[] Decompress(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var ms2 = new MemoryStream())
                {
                    using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        byte[] payload = new byte[100000];
                        int count = 0;

                        while ((count = gzip.Read(payload, 0, payload.Length)) > 0)
                        {
                            ms2.Write(payload, 0, count);
                        }
                        return ms2.ToArray();
                    }
                }
            }
        }
    }
}
