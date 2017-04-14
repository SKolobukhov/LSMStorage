using System.Collections.Concurrent;

namespace LSMStorage.Core
{
    public class MemTable : IMemTable
    {
        private readonly IOpLogWriter opLogWriter;
        private readonly ConcurrentDictionary<string, Item> memoryHash;

        public MemTable(IOpLogWriter opLogWriter)
        {
            this.opLogWriter = opLogWriter;
            memoryHash = new ConcurrentDictionary<string, Item>();
        }

        private void Add(Item item)
        {
            memoryHash.AddOrUpdate(item.Key, item, (key, curItem) => item);
        }

        public void Apply(IOperation operation)
        {
            opLogWriter.Write(operation);
            Add(operation.ToItem());
        }

        public Item Get(string key)
        {
            Item value;
            return memoryHash.TryGetValue(key, out value) ? value : null;
        }
    }
}