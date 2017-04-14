using System.Threading.Tasks;

namespace LSMStorage.Core
{
    internal interface IOpLogApplier
    {
        void Apply(IMemTable memTable);
        Task ApplyAsync(IMemTable memTable);
    }
}