using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using Microsoft.Win32;

namespace ApplicationWatcher.Service.Utils.Services.Wrappers
{
    public class RegistryWrapperService : IRegistryWrapperService
    {
        public T GetRegistryValue<T>(string keyPath, string valueName, RegistryHive hive)
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
            using var key = baseKey.OpenSubKey(keyPath);
            var value = key?.GetValue(valueName);
            return value != null ? (T) value : default;
        }
    }
}
