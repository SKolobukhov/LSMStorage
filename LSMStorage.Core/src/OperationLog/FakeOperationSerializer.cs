using System.IO;
using System.Threading.Tasks;

namespace LSMStorage.Core
{
    public class FakeOperationSerializer : IOperationSerializer 
    {
        public byte Marker { get; }
        public byte[] Serialize(IOperation operation)
        {
            return new byte[0];
        }

        public IOperation Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public Task<IOperation> DeserializeAsync(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}