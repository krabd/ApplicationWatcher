namespace ApplicationWatcher.Service.Utils.Interfaces.Wrappers
{
    public interface IFileWrapperService
    {
        void CreateZip(string source, string destination);

        byte[] ReadAllBytes(string path);

        void Delete(string path);
    }
}
