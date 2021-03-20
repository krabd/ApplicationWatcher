﻿using System;
using ApplicationWatcher.Service.Utils.Interfaces;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace ApplicationWatcher.Service.Utils.Services
{
    public class RegistryService : IRegistryService
    {
        private readonly IRegistryWrapperService _registryWrapperService;
        private readonly ILogger<RegistryService> _logger;

        public RegistryService(IRegistryWrapperService registryWrapperService, ILogger<RegistryService> logger)
        {
            _registryWrapperService = registryWrapperService;
            _logger = logger;
        }

        public string GetRegistryValue(string basePath, string valueName)
        {
            string value = null;

            if (!string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(valueName))
            {
                try
                {
                    using var key = _registryWrapperService.LocalMachineOpenKey(basePath);

                    if (key != null)
                    {
                        value = (string)key.GetValue(valueName);

                        if (value == null)
                        {
                            _logger.LogInformation($"Can not find registry value = {valueName} in path {basePath}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Can not find registry key = {basePath}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error find value in registry by base path = {basePath}, value name = {valueName}", e);
                }
            }
            else
            {
                _logger.LogInformation($"Empty parameters for search in registry base path = {basePath}, value name = {valueName}");
            }

            return value;
        }
    }
}
