namespace ApplicationWatcher.Service.Utils.Interfaces
{
    public interface IRegistryService
    {
        string GetRegistryValue(string basePath, string valueName);
    }
}
