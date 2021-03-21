using System.Diagnostics;
using ApplicationWatcher.Service.Utils.Helpers;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;

namespace ApplicationWatcher.Service.Utils.Services.Wrappers
{
    public class ProcessWrapperService : IProcessWrapperService
    {
        public Process[] GetProcessesByName(string processName)
        {
            return Process.GetProcessesByName(processName);
        }

        public void StartProcessFromWindowService(string exePath)
        {
            //TODO: если откажемся от виндового сервиса, можно использовать нормальный Process.Start(exePath);
            ApplicationLoader.StartProcessAndBypassUAC(exePath, out var procInfo);
        }
    }
}
