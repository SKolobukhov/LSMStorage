using System;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface IOpLogWriter: IDisposable
    {
        void Write(IOperation operation);
        Task WriteAsync(IOperation operation);
    }
}