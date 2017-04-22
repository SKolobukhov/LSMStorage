using System;
using LSMStorage.Core;

namespace LSMStorage.Tests
{
    internal static class Extensions
    {
        public static IOperation ToOperation(this Item item)
        {
            return item.IsTombStone ? (IOperation) new RemoveOperation(item.Key, DateTime.UtcNow.Ticks) : new PutOperation(item.Key, item.Value, DateTime.UtcNow.Ticks);
        }

        public static string Get(this IMemTable memTable, string key, bool deleted = false)
        {
            string value = null;
            var get = new GetOperation(key, v => value = v, deleted);
            memTable.Apply(get);
            return value;
        }
    }
}