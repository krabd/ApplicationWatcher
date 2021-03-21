using Microsoft.Win32;

namespace ApplicationWatcher.Service.Utils.Interfaces.Wrappers
{
    public interface IRegistryWrapperService
    {
        T GetRegistryValue<T>(string keyPath, string valueName, RegistryHive hive);
    }
}
