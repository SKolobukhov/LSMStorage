using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public class RemoveOperationSerializer: IOperationSerializer<RemoveOperation>
    {
        public override byte Marker => 0x52;


        protected override byte[] Serialize(RemoveOperation operation)
        {
            return Enumerable
                .Empty<byte>()
                .Concat(BitConverter.GetBytes(operation.Key.Length))
                .Concat(Encoding.UTF8.GetBytes(operation.Key))
                .ToArray();
        }

        protected override RemoveOperation Deserialize(Stream stream)
        {
            var buffer = stream.EnsureReadBytes(sizeof(int));
            var keyLength = BitConverter.ToInt32(buffer, 0);
            buffer = stream.EnsureReadBytes(keyLength);
            var key = Encoding.UTF8.GetString(buffer);
            return new RemoveOperation(key);
        }

        protected override async Task<RemoveOperation> DeserializeAsync(Stream stream)
        {
            var buffer = await stream.EnsureReadBytesAsync(sizeof(int)).ConfigureAwait(false);
            var keyLength = BitConverter.ToInt32(buffer, 0);
            buffer = await stream.EnsureReadBytesAsync(keyLength).ConfigureAwait(false);
            var key = Encoding.UTF8.GetString(buffer);
            return new RemoveOperation(key);
        }
    }
}