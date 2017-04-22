using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface ISnapshotWriter
    {
        void Write(IMemStorage memStorage, long opLogPosition);
        Task WriteAsync(IMemStorage memStorage, long opLogPosition);
    }
}