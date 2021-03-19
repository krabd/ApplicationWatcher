using System.Threading.Tasks;
using System.Windows;
using Grpc.Core;
using SharedLib.Generated;
using Status = SharedLib.Generated.Status;

namespace ApplicationWatcher.Grpc.Host.Services
{
    public class WatcherService : SharedLib.Generated.WatcherService.WatcherServiceBase
    {
        public override Task<HealthCheckResponse> HealthCheck(Empty request, ServerCallContext context)
        {
            Application.Current.Dispatcher.Invoke(() => { });
            return Task.FromResult(new HealthCheckResponse { Status = Status.Ok });
        }
    }
}