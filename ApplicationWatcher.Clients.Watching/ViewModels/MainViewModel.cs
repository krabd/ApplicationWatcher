using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using ApplicationWatcher.Clients.Utils.ViewModels;
using ApplicationWatcher.Http.Client.Contracts;
using Prism.Commands;
using Sv.ProxyService.Client;

namespace ApplicationWatcher.Clients.Watching.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IApplicationWatcherClient _applicationWatcherClient;

        private string _baseUrl = "http://localhost:5000";
        private bool? _isAlive = null;

        public string BaseUrl
        {
            get => _baseUrl;
            set
            {
                SetProperty(ref _baseUrl, value);
                ((ApplicationWatcherClient)_applicationWatcherClient).BaseUrl = value;
            }
        }

        public bool? IsAlive
        {
            get => _isAlive;
            set => SetProperty(ref _isAlive, value);
        }

        public ICommand RebootCommand { get; }

        public ICommand GetLogsCommand { get; }

        public ICommand HealthCheckCommand { get; }

        public MainViewModel(IApplicationWatcherClient applicationWatcherClient)
        {
            _applicationWatcherClient = applicationWatcherClient;
            ((ApplicationWatcherClient)_applicationWatcherClient).BaseUrl = _baseUrl;

            RebootCommand = new DelegateCommand(OnReboot);
            GetLogsCommand = new DelegateCommand(OnGetLogs);
            HealthCheckCommand = new DelegateCommand(OnHealthCheck);
        }

        private void OnReboot()
        {
            _applicationWatcherClient.RebootAsync();
        }

        private async void OnGetLogs()
        {
            var logs = await _applicationWatcherClient.GetLogsAsync();

            var currentLocation = Process.GetCurrentProcess().MainModule.FileName;
            var destinationPath = Path.Combine(currentLocation.Substring(0, currentLocation.LastIndexOf('\\')), $"{Guid.NewGuid()}.zip");
            using (var destination = File.Create(destinationPath))
            {
                logs.Stream.CopyTo(destination);
            }
            logs.Stream.Close();
        }

        private async void OnHealthCheck()
        {
            IsAlive = null;

            try
            {
                IsAlive = await _applicationWatcherClient.HealthCheckAsync();
            }
            catch (Exception) { }

            Debug.WriteLine($"Heath check result {IsAlive}");
        }
    }
}
