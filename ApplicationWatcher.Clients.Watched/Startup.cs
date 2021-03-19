using System;
using ApplicationWatcher.Grpc.Host.Interfaces;
using ApplicationWatcher.Grpc.Host.Services;
using ApplicationWatcher.WatchedClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationWatcher.WatchedClient
{
    public class Startup
    {
        public void Configure(IServiceCollection services)
        {
            services.AddScoped<MainViewModel>();
            services.AddScoped<IGrpcHostService, GrpcHostService>();
        }

        public IServiceProvider BuildProvider(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }
}
