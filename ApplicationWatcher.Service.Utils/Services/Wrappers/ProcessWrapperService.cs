using System.Diagnostics;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;

namespace ApplicationWatcher.Service.Utils.Services.Wrappers
{
    public class ProcessWrapperService : IProcessWrapperService
    {
        public Process[] GetProcessesByName(string processName)
        {
            return Process.GetProcessesByName(processName);
        }
    }
}
