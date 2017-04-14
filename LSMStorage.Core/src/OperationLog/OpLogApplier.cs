using System.Threading.Tasks;

namespace LSMStorage.Core
{
    internal class OpLogApplier : IOpLogApplier
    {
        private readonly IOpLogReader opLogReader;

        public OpLogApplier(IOpLogReader opLogReader)
        {
            this.opLogReader = opLogReader;
        }

        public void Apply(IMemTable memTable)
        {
            while (opLogReader.CanRead)
            {
                var operation = opLogReader.Read();
                memTable.Apply(operation);
            }
        }

        public async Task ApplyAsync(IMemTable memTable)
        {
            while (opLogReader.CanRead)
            {
                var operation = await opLogReader.ReadAsync().ConfigureAwait(false);
                memTable.Apply(operation);
            }
        }
    }
}