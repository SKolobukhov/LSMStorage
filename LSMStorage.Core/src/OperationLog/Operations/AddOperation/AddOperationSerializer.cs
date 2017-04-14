using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public class AddOperationSerializer : IOperationSerializer<AddOperation>
    {
        public override byte Marker => 0x41;


        protected override byte[] Serialize(AddOperation operation)
        {
            return Enumerable
                .Empty<byte>()
                .Concat(BitConverter.GetBytes(operation.Key.Length))
                .Concat(BitConverter.GetBytes(operation.Value.Length))
                .Concat(Encoding.UTF8.GetBytes(operation.Key))
                .Concat(Encoding.UTF8.GetBytes(operation.Value))
                .ToArray();
        }

        protected override AddOperation Deserialize(Stream stream)
        {
            var buffer = stream.EnsureReadBytes(sizeof(int));
            var keyLength = BitConverter.ToInt32(buffer, 0);
            buffer = stream.EnsureReadBytes(sizeof(int));
            var valueLength = BitConverter.ToInt32(buffer, 0);
            buffer = stream.EnsureReadBytes(keyLength);
            var key = Encoding.UTF8.GetString(buffer);
            buffer = stream.EnsureReadBytes(valueLength);
            var value = Encoding.UTF8.GetString(buffer);
            return new AddOperation(key, value);
        }

        protected override async Task<AddOperation> DeserializeAsync(Stream stream)
        {
            var buffer = await stream.EnsureReadBytesAsync(sizeof(int)).ConfigureAwait(false);
            var keyLength = BitConverter.ToInt32(buffer, 0);
            buffer = await stream.EnsureReadBytesAsync(sizeof(int)).ConfigureAwait(false);
            var valueLength = BitConverter.ToInt32(buffer, 0);
            buffer = await stream.EnsureReadBytesAsync(keyLength).ConfigureAwait(false);
            var key = Encoding.UTF8.GetString(buffer);
            buffer = await stream.EnsureReadBytesAsync(valueLength).ConfigureAwait(false);
            var value = Encoding.UTF8.GetString(buffer);
            return new AddOperation(key, value);
        }
    }
}