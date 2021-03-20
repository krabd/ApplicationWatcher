using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ApplicationWatcher.Grpc.Client.Interfaces;
using ApplicationWatcher.Service.Api.Interfaces;
using ApplicationWatcher.Service.Api.Models.Options;
using ApplicationWatcher.Service.Utils.Helpers;
using ApplicationWatcher.Service.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegistryOptions = ApplicationWatcher.Service.Api.Models.Options.RegistryOptions;

namespace ApplicationWatcher.Service.Api.Services
{
    public class ApplicationWatcherService : IApplicationWatcherService
    {
        private readonly IRegistryService _registryService;
        private readonly IProcessService _processService;
        private readonly IGrpcClientService _grpcClient;

        private readonly RegistryOptions _registryOptions;
        private readonly HealthCheckOptions _healthCheckOptions;
        private readonly GrpcOptions _grpcOptions;

        private readonly ILogger<ApplicationWatcherService> _logger;

        public ApplicationWatcherService(IRegistryService registryService, IProcessService processService, IGrpcClientService grpcClient,
            IOptions<RegistryOptions> registryOptions, IOptions<HealthCheckOptions> healthCheckOptions, IOptions<GrpcOptions> grpcOptions, ILogger<ApplicationWatcherService> logger)
        {
            _registryService = registryService;
            _processService = processService;
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

                    //TODO: если откажемся от виндового сервиса, можно использовать нормальный Process.Start(exePath);
                    ApplicationLoader.StartProcessAndBypassUAC(exePath, out var procInfo);
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
            try
            {
                var logsPath = _registryService.GetRegistryValue(_registryOptions.BasePath, _registryOptions.LogsValue);
                if (!string.IsNullOrEmpty(logsPath))
                {
                    var destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Guid.NewGuid()}.zip");

                    _logger.LogInformation($"Try zip logsPath = {logsPath}");
                    _logger.LogInformation($"Try zip destPath = {destinationPath}");

                    ZipFile.CreateFromDirectory(logsPath, destinationPath);
                    var fileData = File.ReadAllBytes(destinationPath);
                    File.Delete(destinationPath);
                    return fileData;
                }

                _logger.LogInformation($"Try get logs logsPath is empty");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error get logs sv", e);
            }

            return null;
        }

        public async Task<bool> HealthCheck(CancellationToken cancellationToken)
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
