using System;
using System.IO;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    internal static class StreamExtensions
    {
        public static byte EnsureReadByte(this Stream stream)
        {
            Preconditions.EnsureCondition(stream.CanRead, "stream", "Can not read from this stream");
            var read = stream.ReadByte();
            if (read == -1)
            {
                throw new ApplicationException($"Fail to read byte");
            }
            return (byte)read;
        }

        public static byte[] EnsureReadBytes(this Stream stream, int count)
        {
            Preconditions.EnsureCondition(stream.CanRead, "stream", "Can not read from this stream");
            var buffer = new byte[count];
            var read = stream.Read(buffer, 0, count);
            if (read != count)
            {
                throw new ApplicationException($"Fail to read {count} bytes. Read only {read}");
            }
            return buffer;
        }

        public static async Task<byte[]> EnsureReadBytesAsync(this Stream stream, int count)
        {
            Preconditions.EnsureCondition(stream.CanRead, "stream", "Can not read from this stream");
            var buffer = new byte[count];
            var read = await stream.ReadAsync(buffer, 0, count).ConfigureAwait(false);
            if (read != count)
            {
                throw new ApplicationException($"Fail to read {count} bytes. Read only {read}");
            }
            return buffer;
        }
    }
}
