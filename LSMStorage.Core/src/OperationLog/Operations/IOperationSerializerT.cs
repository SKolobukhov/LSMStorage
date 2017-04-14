using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public abstract class IOperationSerializer<TOperation> : IOperationSerializer
        where TOperation : IOperation
    {
        public abstract byte Marker { get; }

        protected abstract byte[] Serialize(TOperation operation);
        protected abstract TOperation Deserialize(Stream stream);
        protected abstract Task<TOperation> DeserializeAsync(Stream stream);

        byte[] IOperationSerializer.Serialize(IOperation operation)
        {
            return new[] { Marker }.Concat(Serialize((TOperation)operation)).ToArray();
        }

        IOperation IOperationSerializer.Deserialize(Stream stream)
        {
            CheckMarker(stream);
            return Deserialize(stream);
        }
        
        async Task<IOperation> IOperationSerializer.DeserializeAsync(Stream stream)
        {
            CheckMarker(stream);
            return await DeserializeAsync(stream).ConfigureAwait(false);
        }

        private void CheckMarker(Stream stream)
        {
            var marker = stream.EnsureReadByte();
            if (marker != Marker)
            {
                throw new ApplicationException($"Can't deserialize block, that start with marker '{marker:X}'");
            }
        }
    }
}