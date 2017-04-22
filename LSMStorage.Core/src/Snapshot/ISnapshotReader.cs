using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface ISnapshotReader
    {
        long Read(IMemStorage memStorage);
        Task<long> ReadAsync(IMemStorage memStorage);
    }
}