using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using Microsoft.Win32;

namespace ApplicationWatcher.Service.Utils.Services.Wrappers
{
    public class RegistryWrapperService : IRegistryWrapperService
    {
        public RegistryKey LocalMachineOpenKey(string path)
        {
            return Registry.LocalMachine.OpenSubKey(path);
        }
    }
}
