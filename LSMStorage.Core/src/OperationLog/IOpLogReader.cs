using System;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface IOpLogReader: IDisposable
    {
        bool CanRead { get; }

        IOperation Read();
        Task<IOperation> ReadAsync();
    }
}