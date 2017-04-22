using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface IMemTable
    {
        void Apply(IOperation operation);
        Task ApplyAsync(IOperation operation);
    }
}