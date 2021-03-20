using Microsoft.Win32;

namespace ApplicationWatcher.Service.Utils.Interfaces.Wrappers
{
    public interface IRegistryWrapperService
    {
        RegistryKey LocalMachineOpenKey(string path);
    }
}
