using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace UOStudio.Common.Core.HashStorage
{
    internal static class IncrementalHashExtensions
    {
        public static async Task<byte[]> ComputeHashAsync(this IncrementalHash algorithm, Stream inputStream, int bufferSize = 4096)
        {
            var buffer = new byte[bufferSize];

            while (true)
            {
                var read = await inputStream.ReadAsync(buffer, 0, bufferSize);

                if (read == 0)
                {
                    break;
                }

                algorithm.AppendData(buffer, 0, read);
            }

            return algorithm.GetHashAndReset();
        }
    }
}
