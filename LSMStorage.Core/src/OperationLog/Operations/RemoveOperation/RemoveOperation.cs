﻿namespace LSMStorage.Core
{
    public class RemoveOperation: IOperation
    {
        public string Key { get; }
        public long Timestamp { get; }


        public RemoveOperation(string key, long timestamp)
        {
            Key = key;
            Timestamp = timestamp;
        }
        
        public void Apply(IMemStorage storage)
        {
            storage.Remove(Key, Timestamp);
        }

        public override string ToString()
        {
            return $"Remove {Key}";
        }
    }
}