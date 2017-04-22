using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public class SnapshotManager : ISnapshotWriter, ISnapshotReader
    {
        private readonly IFile file;

        public SnapshotManager(IFile file)
        {
            this.file = file;
        }


        public void Write(IMemStorage memStorage, long opLogPosition)
        {
            file.DeleteFile();
            using (var stream = file.OpenStream(FileAccess.Write, FileShare.Read))
            {
                var snapshot = BitConverter.GetBytes(opLogPosition)
                    .Concat(memStorage.GetAll(true).SelectMany(SerializeItem))
                    .ToArray();
                stream.Write(snapshot, 0, snapshot.Length);
            }
        }

        public async Task WriteAsync(IMemStorage memStorage, long opLogPosition)
        {
            file.DeleteFile();
            using (var stream = file.OpenStream(FileAccess.Write, FileShare.Read))
            {
                var snapshot = BitConverter.GetBytes(opLogPosition)
                    .Concat(memStorage.GetAll(true).SelectMany(SerializeItem))
                    .ToArray();
                await stream.WriteAsync(snapshot, 0, snapshot.Length).ConfigureAwait(false);
            }
        }

        private byte[] SerializeItem(Item item)
        {
            return Enumerable
                .Empty<byte>()
                .Concat(BitConverter.GetBytes(item.Key.Length))
                .Concat(Encoding.UTF8.GetBytes(item.Key))
                .Concat(BitConverter.GetBytes(item.Timestamp))
                .Concat(BitConverter.GetBytes(item.IsTombStone))
                .Concat(BitConverter.GetBytes(item.Value.Length))
                .Concat(Encoding.UTF8.GetBytes(item.Value))
                .ToArray();
        }

        private void AddToMemStorage(IMemStorage memStorage, string key, string value, long timestamp, bool isDeleted)
        {
            if (isDeleted)
            {
                memStorage.Update(key, value, timestamp - 1);
                memStorage.Remove(key, timestamp);
            }
            else
            {
                memStorage.Update(key, value, timestamp);
            }
        }

        public long Read(IMemStorage memStorage)
        {
            var opLogPosition = 0L;
            using (var stream = file.OpenStream(FileAccess.Write, FileShare.Read))
            {
                if (stream.Position < stream.Length)
                {
                    opLogPosition = stream.EnsureReadInt64();
                }

                while (stream.Position < stream.Length)
                {
                    var keyLength = stream.EnsureReadInt32();
                    var key = stream.EnsureReadString(keyLength);
                    var timestamp = stream.EnsureReadInt64();
                    var isDeleted = stream.EnsureReadBool();
                    var valueLength = stream.EnsureReadInt32();
                    var value = stream.EnsureReadString(valueLength);
                    AddToMemStorage(memStorage, key, value, timestamp, isDeleted);
                }
            }

            return opLogPosition;
        }

        public async Task<long> ReadAsync(IMemStorage memStorage)
        {
            var opLogPosition = 0L;
            using (var stream = file.OpenStream(FileAccess.Write, FileShare.Read))
            {
                if (stream.Position < stream.Length)
                {
                    opLogPosition = stream.EnsureReadInt64();
                }

                while (stream.Position < stream.Length)
                {
                    var keyLength = await stream.EnsureReadInt32Async().ConfigureAwait(false);
                    var key = await stream.EnsureReadStringAsync(keyLength).ConfigureAwait(false);
                    var timestamp = await stream.EnsureReadInt64Async().ConfigureAwait(false);
                    var isDeleted = await stream.EnsureReadBoolAsync().ConfigureAwait(false);
                    var valueLength = await stream.EnsureReadInt32Async().ConfigureAwait(false);
                    var value = await stream.EnsureReadStringAsync(valueLength).ConfigureAwait(false);
                    AddToMemStorage(memStorage, key, value, timestamp, isDeleted);
                }
            }

            return opLogPosition;
        }
    }
}