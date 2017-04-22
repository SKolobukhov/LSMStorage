using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public class PutOperationSerializer : IOperationSerializer<PutOperation>
    {
        public override byte Marker => 0x50;


        protected override byte[] Serialize(PutOperation operation)
        {
            return Enumerable
                .Empty<byte>()
                .Concat(BitConverter.GetBytes(operation.Key.Length))
                .Concat(Encoding.UTF8.GetBytes(operation.Key))
                .Concat(BitConverter.GetBytes(operation.Timestamp))
                .Concat(BitConverter.GetBytes(operation.Value.Length))
                .Concat(Encoding.UTF8.GetBytes(operation.Value))
                .ToArray();
        }

        protected override PutOperation Deserialize(Stream stream)
        {
            var keyLength = stream.EnsureReadInt32();
            var key = stream.EnsureReadString(keyLength);
            var timestamp = stream.EnsureReadInt64();
            var valueLength = stream.EnsureReadInt32();
            var value = stream.EnsureReadString(valueLength);
            return new PutOperation(key, value, timestamp);
        }

        protected override async Task<PutOperation> DeserializeAsync(Stream stream)
        {
            var keyLength = await stream.EnsureReadInt32Async().ConfigureAwait(false);
            var key = await stream.EnsureReadStringAsync(keyLength).ConfigureAwait(false);
            var timestamp = await stream.EnsureReadInt64Async().ConfigureAwait(false);
            var valueLength = await stream.EnsureReadInt32Async().ConfigureAwait(false);
            var value = await stream.EnsureReadStringAsync(valueLength).ConfigureAwait(false);
            return new PutOperation(key, value, timestamp);
        }
    }
}