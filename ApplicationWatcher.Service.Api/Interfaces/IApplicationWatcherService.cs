using System.Threading;
using System.Threading.Tasks;

namespace ApplicationWatcher.Service.Api.Interfaces
{
    public interface IApplicationWatcherService
    {
        void Reboot();

        byte[] GetLogs();

        Task<bool> HealthCheckAsync(CancellationToken cancellationToken);
    }
}
