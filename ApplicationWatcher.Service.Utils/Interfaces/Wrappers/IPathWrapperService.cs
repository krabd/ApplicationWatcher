namespace ApplicationWatcher.Service.Utils.Interfaces.Wrappers
{
    public interface IPathWrapperService
    {
        string? GetFileNameWithoutExtension(string? path);

        string? GetDirectoryName(string? path);
    }
}
