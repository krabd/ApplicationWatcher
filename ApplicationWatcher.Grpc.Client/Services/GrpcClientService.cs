using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationWatcher.Grpc.Client.Interfaces;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using SharedLib.Generated;
using Status = SharedLib.Generated.Status;

namespace ApplicationWatcher.Grpc.Client.Services
{
    public class GrpcClientService : IGrpcClientService
    {
        private readonly ILogger<GrpcClientService> _logger;

        public GrpcClientService(ILogger<GrpcClientService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HealthCheck(Uri address, int timeoutSeconds, CancellationToken cancellationToken)
        {
            try
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                using var channel = GrpcChannel.ForAddress(address);
                var client = new WatcherService.WatcherServiceClient(channel);

                _logger.LogInformation($"Send heath check for address: {address}");

                var result = await client.HealthCheckAsync(new Empty(), deadline: DateTime.UtcNow.AddSeconds(timeoutSeconds), cancellationToken: cancellationToken);

                _logger.LogInformation($"Heath check response status: {result.Status}");

                return result.Status == Status.Ok;
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                _logger.LogInformation($"Heath check timeout error", e);

                return false;
            }
        }
    }
}
