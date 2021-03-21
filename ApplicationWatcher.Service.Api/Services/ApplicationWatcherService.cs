using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ApplicationWatcher.Grpc.Client.Interfaces;
using ApplicationWatcher.Service.Api.Interfaces;
using ApplicationWatcher.Service.Api.Models.Options;
using ApplicationWatcher.Service.Utils.Interfaces;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegistryOptions = ApplicationWatcher.Service.Api.Models.Options.RegistryOptions;

namespace ApplicationWatcher.Service.Api.Services
{
    public class ApplicationWatcherService : IApplicationWatcherService
    {
        private readonly IRegistryService _registryService;
        private readonly IProcessService _processService;
        private readonly IFileWrapperService _fileWrapperService;
        private readonly IGrpcClientService _grpcClient;

        private readonly RegistryOptions _registryOptions;
        private readonly HealthCheckOptions _healthCheckOptions;
        private readonly GrpcOptions _grpcOptions;

        private readonly ILogger<ApplicationWatcherService> _logger;

        public ApplicationWatcherService(IRegistryService registryService, IProcessService processService, IFileWrapperService fileWrapperService,
            IGrpcClientService grpcClient,
            IOptions<RegistryOptions> registryOptions, IOptions<HealthCheckOptions> healthCheckOptions, IOptions<GrpcOptions> grpcOptions, 
            ILogger<ApplicationWatcherService> logger)
        {
            _registryService = registryService;
            _processService = processService;
            _fileWrapperService = fileWrapperService;
            _grpcClient = grpcClient;

            _registryOptions = registryOptions.Value;
            _healthCheckOptions = healthCheckOptions.Value;
            _grpcOptions = grpcOptions.Value;

            _logger = logger;
        }

        public void Reboot()
        {
            try
            {
                var exePath = _registryService.GetRegistryValue(_registryOptions.BasePath, _registryOptions.ExeValue);
                if (!string.IsNullOrEmpty(exePath))
                {
                    _logger.LogInformation($"Try reboot exePath = {exePath}");

                    var process = _processService.GetProcessByExePath(exePath);
                    process?.Kill();

                    _processService.StartProcessFromWindowService(exePath);
                }
                else
                {
                    _logger.LogInformation($"Can not find exe path");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error reboot sv", e);
            }
        }

        public byte[] GetLogs()
        {
            byte[] fileData = null;

            try
            {
                var logsPath = _registryService.GetRegistryValue(_registryOptions.BasePath, _registryOptions.LogsValue);
                if (!string.IsNullOrEmpty(logsPath))
                {
                    var destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Guid.NewGuid()}.zip");

                    _logger.LogInformation($"Create zip logsPath = {logsPath} destPath = {destinationPath}");

                    _fileWrapperService.CreateZip(logsPath, destinationPath);

                    _logger.LogInformation($"Read zip data path = {destinationPath}");

                    fileData = _fileWrapperService.ReadAllBytes(destinationPath);

                    _logger.LogInformation($"Delete zip path = {destinationPath}");

                    _fileWrapperService.Delete(destinationPath);
                }

                _logger.LogInformation($"Try get logs logsPath is empty");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error get logs sv", e);
            }

            return fileData;
        }

        public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Health check Sv");

            try
            {
                return await _grpcClient.HealthCheck(_grpcOptions.GetUri(), _healthCheckOptions.TimeoutSeconds, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error health check Sv", e);
                return false;
            }
        }
    }
}
