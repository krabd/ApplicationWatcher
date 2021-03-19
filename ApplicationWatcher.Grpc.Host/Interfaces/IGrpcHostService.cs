using System.Threading.Tasks;

namespace ApplicationWatcher.Grpc.Host.Interfaces
{
    public interface IGrpcHostService
    {
        Task StartServer(string host, int port);

        Task StopServer();
    }
}
