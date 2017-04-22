using System.IO;

namespace LSMStorage.Core
{
    public class File : IFile
    {
        public readonly string Path;

        public bool IsExist => System.IO.File.Exists(Path);

        public File(string path)
        {
            Path = path;
        }

        public Stream OpenStream()
        {
            return System.IO.File.Open(Path, FileMode.OpenOrCreate);
        }

        public Stream OpenStream(FileAccess fileAccess)
        {
            return System.IO.File.Open(Path, FileMode.OpenOrCreate, fileAccess);
        }

        public Stream OpenStream(FileAccess fileAccess, FileShare fileShare)
        {
            return System.IO.File.Open(Path, FileMode.OpenOrCreate, fileAccess, fileShare);
        }

        public Stream OpenStream(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return System.IO.File.Open(Path, fileMode, fileAccess, fileShare);
        }

        public void DeleteFile()
        {
            if (IsExist)
            {
                System.IO.File.Delete(Path);
            }
        }
    }
}