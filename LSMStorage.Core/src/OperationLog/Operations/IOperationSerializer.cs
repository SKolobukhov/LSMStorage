using System.IO;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public interface IOperationSerializer
    {
        byte Marker { get; }

        byte[] Serialize(IOperation operation);
        IOperation Deserialize(Stream stream);
        Task<IOperation> DeserializeAsync(Stream stream);
    }
}