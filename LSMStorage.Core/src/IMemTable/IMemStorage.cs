using System.Collections.Generic;

namespace LSMStorage.Core
{
    public interface IMemStorage
    {
        bool Update(string key, string value, long timestamp);
        bool Remove(string key, long timestamp);
        string Get(string key);
        string Get(string key, bool deleted);
        IEnumerable<Item> GetAll();
        IEnumerable<Item> GetAll(bool deleted);
    }
}