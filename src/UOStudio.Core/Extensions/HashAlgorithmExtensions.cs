using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UOStudio.Core.Extensions
{
    public static class HashAlgorithmExtensions
    {
        public static async Task<byte[]> ComputeHashAsync(
            this HashAlgorithm hashAlgorithm,
            IEnumerable<FileInfo> files,
            bool includePaths = true)
        {
            await using var cs = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write);
            foreach (var file in files)
            {
                if (includePaths)
                {
                    var pathBytes = Encoding.UTF8.GetBytes(file.FullName);
                    cs.Write(pathBytes, 0, pathBytes.Length);
                }

                await using var fs = file.OpenRead();
                await fs.CopyToAsync(cs);
            }

            cs.FlushFinalBlock();

            return hashAlgorithm.Hash;
        }
    }
}
