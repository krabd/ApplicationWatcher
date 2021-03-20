using System.IO;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;

namespace ApplicationWatcher.Service.Utils.Services.Wrappers
{
    public class PathWrapperService : IPathWrapperService
    {
        public string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}
