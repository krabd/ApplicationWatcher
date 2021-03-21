using System.IO;
using System.IO.Compression;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;

namespace ApplicationWatcher.Service.Utils.Services.Wrappers
{
    public class FileWrapperService : IFileWrapperService
    {
        public void CreateZip(string source, string destination)
        {
            ZipFile.CreateFromDirectory(source, destination);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }
    }
}
