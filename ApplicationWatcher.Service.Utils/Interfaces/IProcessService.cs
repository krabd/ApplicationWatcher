using System.Diagnostics;

namespace ApplicationWatcher.Service.Utils.Interfaces
{
    public interface IProcessService
    {
        Process GetProcessByExePath(string exePath);

        void StartProcessFromWindowService(string exePath);
    }
}
