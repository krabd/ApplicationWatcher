using System;
using ApplicationWatcher.Clients.Watching.ViewModels;
using ApplicationWatcher.Http.Client.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Sv.ProxyService.Client;

namespace ApplicationWatcher.Clients.Watching
{
    public class Startup
    {
        public void Configure(IServiceCollection services)
        {
            services.AddScoped<MainViewModel>();
            services.AddScoped<IApplicationWatcherClient>(provider => new ApplicationWatcherClient("http://localhost:5000"));
        }

        public IServiceProvider BuildProvider(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }
}
