using System.Threading;
using System.Threading.Tasks;

namespace ApplicationWatcherService.Api.Interfaces
{
    public interface IApplicationWatcherService
    {
        void Reboot();

        byte[] GetLogs();

        Task<bool> HealthCheck(CancellationToken cancellationToken);
    }
}
