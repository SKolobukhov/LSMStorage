using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface IOpLogWriter
    {
        void Write(IOperation operation);
        Task WriteAsync(IOperation operation);
    }
}