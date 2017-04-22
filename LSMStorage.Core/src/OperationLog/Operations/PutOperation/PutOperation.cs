namespace LSMStorage.Core
{
    public class PutOperation: IOperation
    {
        public string Key { get; }
        public string Value { get; }
        public long Timestamp { get; }

        public PutOperation(string key, string value, long timestamp)
        {
            Key = key;
            Value = value;
            Timestamp = timestamp;
        }

        public void Apply(IMemStorage storage)
        {
            storage.Update(Key, Value, Timestamp);
        }

        public override string ToString()
        {
            return $"Put ({Key}, {Value})";
        }
    }
}