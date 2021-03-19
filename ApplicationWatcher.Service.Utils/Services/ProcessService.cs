using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ApplicationWatcher.Service.Utils.Interfaces;

namespace ApplicationWatcher.Service.Utils.Services
{
    public class ProcessService : IProcessService
    {
        public Process GetProcessByExePath(string exePath)
        {
            return Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exePath).ToLower())
                .FirstOrDefault(p => p.MainModule != null
                                     && p.MainModule.FileName.StartsWith(Path.GetDirectoryName(exePath), StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
