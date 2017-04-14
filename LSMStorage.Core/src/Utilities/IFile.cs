using System.IO;

namespace LSMStorage.Core
{
    public interface IFile
    {
        Stream GetStream();
    }
}