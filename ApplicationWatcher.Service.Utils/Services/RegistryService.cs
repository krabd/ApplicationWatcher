using ApplicationWatcher.Service.Utils.Interfaces;
using Microsoft.Win32;

namespace ApplicationWatcher.Service.Utils.Services
{
    public class RegistryService : IRegistryService
    {
        public T GetRegistryValue<T>(string basePath, string valueName, RegistryHive registryHive)
        {
            using var baseKey = RegistryKey.OpenBaseKey(registryHive, RegistryView.Default);
            using var subKey = baseKey.OpenSubKey(basePath);
            var value = subKey?.GetValue(valueName);
            return value != null ? (T)value : default;
        }
    }
}
