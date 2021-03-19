using System.Threading.Tasks;
using ApplicationWatcher.Grpc.Host.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ApplicationWatcher.Grpc.Host.Services
{
    public class GrpcHostService : IGrpcHostService
    {
        private readonly ILogger<GrpcHostService> _logger;

        private Server _server;

        public GrpcHostService()
        {
            _logger = NullLogger<GrpcHostService>.Instance;
        }

        public Task StartServer(string host, int port)
        {
            return Task.Run(() =>
            {
                if (_server != null)
                {
                    _logger.LogInformation($"Server already started");
                    return;
                }

                _server = new Server();
                _server.Services.Add(SharedLib.Generated.WatcherService.BindService(new WatcherService()));
                _server.Ports.Add(new ServerPort(host, port, ServerCredentials.Insecure));
                _server.Start();

                _logger.LogInformation($"Server started host: {host} port: {port}");
            });
        }

        public Task StopServer()
        {
            return Task.Run(async () =>
            {
                if (_server == null) return;

                await _server.ShutdownAsync();
                _server = null;

                _logger.LogInformation("Server stopped");
            });
        }
    }
}
