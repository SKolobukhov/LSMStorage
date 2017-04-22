using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public class OpLogManager : IOpLogReader, IOpLogWriter
    {
        private readonly Stream readStream;
        private readonly Stream writeStream;
        private readonly Mutex readMutex = new Mutex();
        private readonly Mutex writeMutex = new Mutex();
        private readonly IOperationSerializer serializer;

        public long Position => writeStream.Position;

        public OpLogManager(IFile opLogFile, IOperationSerializer serializer)
        {
            readStream = opLogFile.OpenStream(FileAccess.Read, FileShare.ReadWrite);
            writeStream = opLogFile.OpenStream(FileAccess.Write, FileShare.Read);
            this.serializer = serializer;
        }

        public bool CanRead => readStream.Position < readStream.Length;

        public IOperation Read()
        {
            readMutex.WaitOne();
            var result = serializer.Deserialize(readStream);
            readMutex.ReleaseMutex();
            return result;
        }

        public async Task<IOperation> ReadAsync()
        {
            readMutex.WaitOne();
            var result = await serializer.DeserializeAsync(readStream).ConfigureAwait(false);
            readMutex.ReleaseMutex();
            return result;
        }
        
        public void Write(IOperation operation)
        {
            var bytes = serializer.Serialize(operation);
            if (bytes.Length == 0)
            {
                return;
            }

            writeMutex.WaitOne();
            writeStream.Write(bytes, 0, bytes.Length);
            writeStream.Flush();
            writeMutex.ReleaseMutex();
        }

        public async Task WriteAsync(IOperation operation)
        {
            var bytes = serializer.Serialize(operation);
            if (bytes.Length == 0)
            {
                return;
            }

            writeMutex.WaitOne();
            await writeStream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            await writeStream.FlushAsync().ConfigureAwait(false);
            writeMutex.ReleaseMutex();
        }

        public void Dispose()
        {
            readMutex.Dispose();
            readStream.Dispose();
            writeStream.Dispose();
        }
    }
}