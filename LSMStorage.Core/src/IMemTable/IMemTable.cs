using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface IMemTable: IEnumerable<Item>
    {
        void Apply(IOperation operation);
        Task ApplyAsync(IOperation operation);
        
        Item Get(string key);
    }
}