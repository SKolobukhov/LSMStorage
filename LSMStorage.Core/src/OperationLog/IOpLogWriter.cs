using System;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface IOpLogWriter: IDisposable
    {
        long Position { get; }

        void Write(IOperation operation);
        Task WriteAsync(IOperation operation);
    }
}