using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public class OpLogManager : IOpLogReader, IOpLogWriter
    {
        private long readOffset;
        private long writeOffset;
        private readonly IFile opLogFile;
        private readonly IOperationSerializer serializer;
        private readonly Mutex mutex = new Mutex();

        public OpLogManager(IFile opLogFile, IOperationSerializer serializer)
        {
            this.opLogFile = opLogFile;
            this.serializer = serializer;
            using (var stream = opLogFile.GetStream())
            {
                Preconditions.EnsureCondition(stream.CanRead, "opLogFile", "Can not read from stream");
                Preconditions.EnsureCondition(stream.CanWrite, "opLogFile", "Can not write to stream");
                Preconditions.EnsureCondition(stream.CanSeek, "opLogFile", "Can not seek on stream");
            }
        }

        public bool CanRead
        {
            get
            {
                var result = false;

                mutex.WaitOne();
                using (var stream = opLogFile.GetStream())
                {
                    result = readOffset < stream.Length;
                }
                mutex.ReleaseMutex();

                return result;
            }
        }

        public IOperation Read()
        {
            IOperation result;
            mutex.WaitOne();

            using (var stream = opLogFile.GetStream())
            {
                stream.Seek(readOffset, SeekOrigin.Begin);
                result = serializer.Deserialize(stream);
                readOffset = stream.Position;
            }
            mutex.ReleaseMutex();
            return result;
        }

        public async Task<IOperation> ReadAsync()
        {
            IOperation result;
            mutex.WaitOne();

            using (var stream = opLogFile.GetStream())
            {
                stream.Seek(readOffset, SeekOrigin.Begin);
                result = await serializer.DeserializeAsync(stream).ConfigureAwait(false);
                readOffset = stream.Position;
            }
            mutex.ReleaseMutex();
            return result;
        }

        public void Write(IOperation operation)
        {
            mutex.WaitOne();
            using (var stream = opLogFile.GetStream())
            {
                stream.Seek(writeOffset, SeekOrigin.Begin);
                var bytes = serializer.Serialize(operation);
                stream.Write(bytes, 0, bytes.Length);
                writeOffset = stream.Position;
            }
            mutex.ReleaseMutex();
        }

        public async Task WriteAsync(IOperation operation)
        {
            mutex.WaitOne();
            using (var stream = opLogFile.GetStream())
            {
                stream.Seek(writeOffset, SeekOrigin.Begin);
                var bytes = serializer.Serialize(operation);
                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                writeOffset = stream.Position;
            }
            mutex.ReleaseMutex();
        }
    }
}