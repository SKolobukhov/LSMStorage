using System;

namespace LSMStorage.Core
{
    public class GetOperation : IOperation
    {
        public long Timestamp { get; }
        public string Key { get; }
        public bool Deleted { get; }
        private readonly Action<string> callback;

        public GetOperation(string key, Action<string> callback, bool deleted = false, long timestamp = 0)
        {
            Timestamp = timestamp;
            Key = key;
            Deleted = deleted;
            this.callback = callback;
        }

        public void Apply(IMemStorage storage)
        {
            callback(storage.Get(Key, Deleted));
        }

        public override string ToString()
        {
            return $"Get {Key}(deleted: {Deleted})";
        }
    }
}