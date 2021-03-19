using Microsoft.Win32;

namespace ApplicationWatcher.Service.Utils.Interfaces
{
    public interface IRegistryService
    {
        T GetRegistryValue<T>(string basePath, string valueName, RegistryHive registryHive);
    }
}
