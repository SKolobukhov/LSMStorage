﻿using System;
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
                .Concat(BitConverter.GetBytes(operation.Timestamp))
                .ToArray();
        }

        protected override RemoveOperation Deserialize(Stream stream)
        {
            var keyLength = stream.EnsureReadInt32();
            var key = stream.EnsureReadString(keyLength);
            var timestamp = stream.EnsureReadInt64();
            return new RemoveOperation(key, timestamp);
        }

        protected override async Task<RemoveOperation> DeserializeAsync(Stream stream)
        {
            var keyLength = await stream.EnsureReadInt32Async().ConfigureAwait(false);
            var key = await stream.EnsureReadStringAsync(keyLength).ConfigureAwait(false);
            var timestamp = await stream.EnsureReadInt64Async().ConfigureAwait(false);
            return new RemoveOperation(key, timestamp);
        }
    }
}