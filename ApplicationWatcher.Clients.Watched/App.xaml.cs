using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using ApplicationWatcher.Clients.Utils.Extensions;
using ApplicationWatcher.WatchedClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationWatcher.WatchedClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                DispatcherUnhandledException += ProcessUnhandledException;

                var serviceCollection = new ServiceCollection();
                var startupService = new Startup();
                startupService.Configure(serviceCollection);

                startupService.Configure(serviceCollection);
                var provider = startupService.BuildProvider(serviceCollection);
                var main = provider.Resolve<MainViewModel>();
                var window = new MainWindow { DataContext = main };
                Current.MainWindow = window;
                window.Show();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Environment.Exit(1);
            }

            base.OnStartup(e);
        }

        private void ProcessUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.WriteLine($"Catch unhandled exception {e.Exception.Message}");
            e.Handled = true;
        }
    }
}
