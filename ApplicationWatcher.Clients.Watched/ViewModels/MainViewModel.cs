using System.Threading;
using System.Windows.Input;
using ApplicationWatcher.Clients.Utils.ViewModels;
using ApplicationWatcher.Grpc.Host.Interfaces;
using Prism.Commands;

namespace ApplicationWatcher.WatchedClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IGrpcHostService _grpcHostService;

        public ICommand StartServerCommand { get; }

        public ICommand StopServerCommand { get; }

        public ICommand SimulateFreezeCommand { get; }

        public MainViewModel(IGrpcHostService grpcHostService)
        {
            _grpcHostService = grpcHostService;

            StartServerCommand = new DelegateCommand(OnStartServer);
            StopServerCommand = new DelegateCommand(OnStopServer);
            SimulateFreezeCommand = new DelegateCommand(OnSimulateFreeze);
        }

        private void OnStartServer()
        {
            _grpcHostService.StartServer("localhost", 50051);
        }

        private void OnStopServer()
        {
            _grpcHostService.StopServer();
        }

        private void OnSimulateFreeze()
        {
            Thread.Sleep(10000);
        }
    }
}
