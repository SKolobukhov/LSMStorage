using System.IO;

namespace LSMStorage.Core
{
    public class File : IFile
    {
        public readonly string Path;

        public File(string path)
        {
            Path = path;
        }

        public Stream GetStream()
        {
            return System.IO.File.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
    }
}