using System.Diagnostics;

namespace ApplicationWatcherService.Utils.Interfaces
{
    public interface IProcessService
    {
        Process GetProcessByExePath(string exePath);
    }
}
