using System.Diagnostics;

namespace ApplicationWatcher.Service.Utils.Interfaces.Wrappers
{
    public interface IProcessWrapperService
    {
        Process[] GetProcessesByName(string processName);

        void StartProcessFromWindowService(string exePath);
    }
}
