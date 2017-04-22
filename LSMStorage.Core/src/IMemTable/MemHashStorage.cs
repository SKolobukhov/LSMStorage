using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LSMStorage.Core
{
    public class MemHashStorage : IMemStorage
    {
        private readonly Dictionary<string, Item> storage = new Dictionary<string, Item>();


        public bool Update(string key, string value, long timestamp)
        {
            var newItem = Item.CreateItem(key, value, timestamp);
            lock (storage)
            {
                Item oldItem;
                if (!storage.TryGetValue(key, out oldItem))
                {
                    storage.Add(key, newItem);
                    return true;
                }

                if (oldItem.Timestamp >= timestamp)
                {
                    return false;
                }

                storage[key] = newItem;
            }
            return true;
        }

        public bool Remove(string key, long timestamp)
        {
            var createdTombStone = Item.CreateTombStone(Item.CreateItem(key, string.Empty, timestamp), timestamp);
            lock (storage)
            {
                Item oldItem;
                if (!storage.TryGetValue(key, out oldItem))
                {
                    storage.Add(key, createdTombStone);
                    return true;
                }

                if (oldItem.Timestamp >= timestamp)
                {
                    return false;
                }

                storage[key] = Item.CreateTombStone(oldItem, timestamp);
            }
            return true;
        }

        public string Get(string key)
        {
            return Get(key, false);
        }

        public string Get(string key, bool deleted)
        {
            Item item;
            lock (storage)
            {
                storage.TryGetValue(key, out item);
            }
            return deleted || item != null && !item.IsTombStone ? item?.Value : null;
        }

        public IEnumerable<Item> GetAll()
        {
            return GetAll(false);
        }

        public IEnumerable<Item> GetAll(bool deleted)
        {
            Item[] items;
            lock (storage)
            {
                items = storage
                    .Select(p => p.Value)
                    .ToArray();
            }

            return items.Where(item => deleted || !item.IsTombStone);
        }
    }
}