using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationWatcher.Grpc.Client.Interfaces
{
    public interface IGrpcClientService
    {
        Task<bool> HealthCheck(Uri address, int timeoutSeconds, CancellationToken cancellationToken);
    }
}
