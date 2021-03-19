using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ApplicationWatcherService.Api.Interfaces;
using ApplicationWatcherService.Api.Models.Options;
using ApplicationWatcherService.Utils.Helpers;
using ApplicationWatcherService.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using RegistryOptions = ApplicationWatcherService.Api.Models.Options.RegistryOptions;

namespace ApplicationWatcherService.Api.Services
{
    public class ApplicationWatcherService : IApplicationWatcherService
    {
        private readonly IRegistryService _registryService;
        private readonly IProcessService _processService;
        private readonly ISvGrpcClientService _svGrpcClient;

        private readonly RegistryOptions _registryOptions;
        private readonly HealthCheckOptions _healthCheckOptions;
        private readonly GrpcOptions _grpcOptions;

        private readonly ILogger<ApplicationWatcherService> _logger;

        public ApplicationWatcherService(IRegistryService registryService, IProcessService processService, ISvGrpcClientService svGrpcClient,
            IOptions<RegistryOptions> registryOptions, IOptions<HealthCheckOptions> healthCheckOptions, IOptions<GrpcOptions> grpcOptions, ILogger<ApplicationWatcherService> logger)
        {
            _registryService = registryService;
            _processService = processService;
            _svGrpcClient = svGrpcClient;

            _registryOptions = registryOptions.Value;
            _healthCheckOptions = healthCheckOptions.Value;
            _grpcOptions = grpcOptions.Value;

            _logger = logger;
        }

        public void Reboot()
        {
            try
            {
                var exePath = _registryService.GetRegistryValue<string>(_registryOptions.BasePath, _registryOptions.ExeValue, RegistryHive.LocalMachine);
                if (string.IsNullOrEmpty(exePath))
                {
                    _logger.LogInformation($"Try reboot exePath is empty");
                    return;
                }

                _logger.LogInformation($"Try reboot exePath = {exePath}");

                var process = _processService.GetProcessByExePath(exePath);
                process?.Kill();

                //TODO: если откажемся от виндового сервиса, можно использовать нормальный Process.Start(exePath);
                ApplicationLoader.StartProcessAndBypassUAC(exePath, out var procInfo);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error reboot sv", e);
            }
        }

        public byte[] GetLogs()
        {
            try
            {
                var logsPath = _registryService.GetRegistryValue<string>(_registryOptions.BasePath, _registryOptions.LogsValue, RegistryHive.LocalMachine);
                if (string.IsNullOrEmpty(logsPath))
                {
                    _logger.LogInformation($"Try get logs logsPath is empty");
                    return null;
                }

                var destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Guid.NewGuid()}.zip");

                _logger.LogInformation($"Try zip logsPath = {logsPath}");
                _logger.LogInformation($"Try zip destPath = {destinationPath}");

                ZipFile.CreateFromDirectory(logsPath, destinationPath);
                var fileData = File.ReadAllBytes(destinationPath);
                File.Delete(destinationPath);
                return fileData;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error get logs sv", e);
                return null;
            }
        }

        public async Task<bool> HealthCheck(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Health check Sv");

            try
            {
                return await _svGrpcClient.HealthCheckSv(_grpcOptions.GetUri(), _healthCheckOptions.TimeoutSeconds, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error health check Sv", e);
                return false;
            }
        }
    }
}
