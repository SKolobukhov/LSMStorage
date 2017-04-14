using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface IOpLogReader
    {
        bool CanRead { get; }

        IOperation Read();
        Task<IOperation> ReadAsync();
    }
}