using System;
using System.Diagnostics;
using System.Linq;
using ApplicationWatcher.Service.Utils.Interfaces;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using Microsoft.Extensions.Logging;

namespace ApplicationWatcher.Service.Utils.Services
{
    public class ProcessService : IProcessService
    {
        private readonly IPathWrapperService _pathWrapperService;
        private readonly IProcessWrapperService _processWrapperService;
        private readonly ILogger<ProcessService> _logger;

        public ProcessService(IPathWrapperService pathWrapperService, IProcessWrapperService processWrapperService, ILogger<ProcessService> logger)
        {
            _pathWrapperService = pathWrapperService;
            _processWrapperService = processWrapperService;
            _logger = logger;
        }

        public Process GetProcessByExePath(string exePath)
        {
            Process process = null;

            if (!string.IsNullOrEmpty(exePath))
            {
                try
                {
                    var fileName = _pathWrapperService.GetFileNameWithoutExtension(exePath);
                    var directoryName = _pathWrapperService.GetDirectoryName(exePath);
                    if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(directoryName))
                    {
                        _logger.LogInformation($"Exe path is invalid. Path: {exePath}");
                    }
                    else
                    {
                        var processesByName = _processWrapperService.GetProcessesByName(fileName.ToLower());
                        if (!processesByName.Any())
                        {
                            _logger.LogInformation($"Can not find processes by name {fileName}");
                        }
                        else
                        {
                            var processes = processesByName.Where(p => p.MainModule != null && p.MainModule.FileName.StartsWith(directoryName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                            if (!processes.Any())
                            {
                                _logger.LogInformation($"Can not find process by exe path {exePath}");
                            }
                            else if (processes.Count() > 1)
                            {
                                _logger.LogInformation($"Find more then one process by exe path {exePath}");
                            }
                            else
                            {
                                process = processes.Single();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error find process by exe path = {exePath}", e);
                }
            }
            else
            {
                _logger.LogInformation("Exe path is empty");
            }

            return process;
        }

        public void StartProcessFromWindowService(string exePath)
        {
            try
            {
                _logger.LogError($"Try start process by exe path = {exePath}");

                _processWrapperService.StartProcessFromWindowService(exePath);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error start process by exe path = {exePath}", e);
            }
        }
    }
}
