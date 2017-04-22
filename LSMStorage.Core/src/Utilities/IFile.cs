using System.IO;

namespace LSMStorage.Core
{
    public interface IFile
    {
        Stream OpenStream();
        Stream OpenStream(FileAccess fileAccess);
        Stream OpenStream(FileAccess fileAccess, FileShare fileShare);
        Stream OpenStream(FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
        void DeleteFile();
    }
}