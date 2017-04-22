using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    internal static class StreamExtensions
    {
        public static byte[] EnsureReadBytes(this Stream stream, int count)
        {
            Preconditions.EnsureCondition(stream.CanRead, "stream", "Can not read from stream");
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
            Preconditions.EnsureCondition(stream.CanRead, "stream", "Can not read from stream");
            var buffer = new byte[count];
            var read = await stream.ReadAsync(buffer, 0, count).ConfigureAwait(false);
            if (read != count)
            {
                throw new ApplicationException($"Fail to read {count} bytes. Read only {read}");
            }
            return buffer;
        }

        /// <summary>
        /// Realy read byte
        /// </summary>
        public static bool EnsureReadBool(this Stream stream)
        {
            return stream.EnsureReadByte() > 0;
        }

        /// <summary>
        /// Realy read byte
        /// </summary>
        public static bool EnsureReadBool(this Stream stream, int bitIndex)
        {
            Preconditions.EnsureArgumentRange(bitIndex >= 0 && bitIndex < 8, nameof(bitIndex), $"Invalid index for bit: {bitIndex}");
            var b = stream.EnsureReadByte();
            return (b & (1 << bitIndex)) > 0;
        }

        /// <summary>
        /// Realy read byte
        /// </summary>
        public static async Task<bool> EnsureReadBoolAsync(this Stream stream)
        {
            return await stream.EnsureReadByteAsync().ConfigureAwait(false) > 0;
        }

        /// <summary>
        /// Realy read byte
        /// </summary>
        public static async Task<bool> EnsureReadBoolAsync(this Stream stream, int bitIndex)
        {
            Preconditions.EnsureArgumentRange(bitIndex >= 0 && bitIndex < 8, nameof(bitIndex), $"Invalid index for bit: {bitIndex}");
            var b = await stream.EnsureReadByteAsync().ConfigureAwait(false);
            return (b & (1 << bitIndex)) > 0;
        }

        public static byte EnsureReadByte(this Stream stream)
        {
            Preconditions.EnsureCondition(stream.CanRead, "stream", "Can not read from stream");
            var read = stream.ReadByte();
            if (read == -1)
            {
                throw new ApplicationException($"Fail to read byte");
            }
            return (byte)read;
        }

        public static async Task<byte> EnsureReadByteAsync(this Stream stream)
        {
            Preconditions.EnsureCondition(stream.CanRead, "stream", "Can not read from stream");
            var buffer = new byte[1];
            var read = await stream.ReadAsync(buffer, 0, 1).ConfigureAwait(false);
            if (read != 1)
            {
                throw new ApplicationException($"Fail to read byte");
            }
            return buffer.First();
        }

        public static short EnsureReadInt16(this Stream stream)
        {
            var buffer = stream.EnsureReadBytes(sizeof(short));
            return BitConverter.ToInt16(buffer, 0);
        }

        public static async Task<short> EnsureReadInt16Async(this Stream stream)
        {
            var buffer = await stream.EnsureReadBytesAsync(sizeof(short)).ConfigureAwait(false);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static ushort EnsureReadUInt16(this Stream stream)
        {
            var buffer = stream.EnsureReadBytes(sizeof(ushort));
            return BitConverter.ToUInt16(buffer, 0);
        }

        public static async Task<ushort> EnsureReadUInt16Async(this Stream stream)
        {
            var buffer = await stream.EnsureReadBytesAsync(sizeof(ushort)).ConfigureAwait(false);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public static int EnsureReadInt32(this Stream stream)
        {
            var buffer = stream.EnsureReadBytes(sizeof(int));
            return BitConverter.ToInt32(buffer, 0);
        }

        public static async Task<int> EnsureReadInt32Async(this Stream stream)
        {
            var buffer = await stream.EnsureReadBytesAsync(sizeof(int)).ConfigureAwait(false);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static uint EnsureReadUInt32(this Stream stream)
        {
            var buffer = stream.EnsureReadBytes(sizeof(uint));
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static async Task<uint> EnsureReadUInt32Async(this Stream stream)
        {
            var buffer = await stream.EnsureReadBytesAsync(sizeof(uint)).ConfigureAwait(false);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static long EnsureReadInt64(this Stream stream)
        {
            var buffer = stream.EnsureReadBytes(sizeof(long));
            return BitConverter.ToInt64(buffer, 0);
        }

        public static async Task<long> EnsureReadInt64Async(this Stream stream)
        {
            var buffer = await stream.EnsureReadBytesAsync(sizeof(long)).ConfigureAwait(false);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static ulong EnsureReadUInt64(this Stream stream)
        {
            var buffer = stream.EnsureReadBytes(sizeof(ulong));
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static async Task<ulong> EnsureReadUInt64Async(this Stream stream)
        {
            var buffer = await stream.EnsureReadBytesAsync(sizeof(ulong)).ConfigureAwait(false);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static string EnsureReadString(this Stream stream, int length)
        {
            return stream.EnsureReadString(length, Encoding.UTF8);
        }

        public static string EnsureReadString(this Stream stream, int length, Encoding encoding)
        {
            var buffer = stream.EnsureReadBytes(length);
            return encoding.GetString(buffer);
        }

        public static Task<string> EnsureReadStringAsync(this Stream stream, int length)
        {
            return stream.EnsureReadStringAsync(length, Encoding.UTF8);
        }

        public static async Task<string> EnsureReadStringAsync(this Stream stream, int length, Encoding encoding)
        {
            var buffer = await stream.EnsureReadBytesAsync(length).ConfigureAwait(false);
            return encoding.GetString(buffer);
        }
    }
}
