using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public class MemTable : IMemTable
    {
        private readonly Mutex mutex = new Mutex();
        private readonly IMemStorage memStorage;
        private readonly IOpLogWriter opLogWriter;
        private readonly ISnapshotWriter snapshotWriter;

        private ulong operationNumber;
        private readonly ulong snapshotPeriod;

        public MemTable(IOpLogWriter opLogWriter, ISnapshotWriter snapshotWriter, ulong snapshotPeriod)
            : this(new MemHashStorage(), opLogWriter, snapshotWriter, snapshotPeriod)
        { }

        public MemTable(IMemStorage memStorage, IOpLogWriter opLogWriter, ISnapshotWriter snapshotWriter, ulong snapshotPeriod)
        {
            this.memStorage = memStorage;
            this.opLogWriter = opLogWriter;
            this.snapshotWriter = snapshotWriter;
            this.snapshotPeriod = snapshotPeriod;
        }

        public void Apply(IOperation operation)
        {
            mutex.WaitOne();
            operationNumber++;
            if (operationNumber == snapshotPeriod)
            {
                operationNumber = 0;
                snapshotWriter.Write(memStorage, opLogWriter.Position);
            }
            mutex.ReleaseMutex();

            opLogWriter.Write(operation);
            operation.Apply(memStorage);
        }

        public async Task ApplyAsync(IOperation operation)
        {
            mutex.WaitOne();
            operationNumber++;
            if (operationNumber == snapshotPeriod)
            {
                operationNumber = 0;
                await snapshotWriter.WriteAsync(memStorage, opLogWriter.Position);
            }
            mutex.ReleaseMutex();

            await opLogWriter.WriteAsync(operation).ConfigureAwait(false);
            operation.Apply(memStorage);
        }
    }
}