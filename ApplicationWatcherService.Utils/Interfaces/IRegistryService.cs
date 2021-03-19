using Microsoft.Win32;

namespace ApplicationWatcherService.Utils.Interfaces
{
    public interface IRegistryService
    {
        T GetRegistryValue<T>(string basePath, string valueName, RegistryHive registryHive);
    }
}
