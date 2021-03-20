using ApplicationWatcher.Grpc.Client.Interfaces;
using ApplicationWatcher.Grpc.Client.Services;
using ApplicationWatcher.Service.Api.Interfaces;
using ApplicationWatcher.Service.Api.Models.Options;
using ApplicationWatcher.Service.Api.Services.HostedServices;
using ApplicationWatcher.Service.Utils.Interfaces;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using ApplicationWatcher.Service.Utils.Services;
using ApplicationWatcher.Service.Utils.Services.Wrappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApplicationWatcher.Service.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerDocument(settings =>
            {
                settings.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Application watcher service API";
                };
            });

            services.Configure<RegistryOptions>(Configuration.GetSection(RegistryOptions.Registry));
            services.Configure<HealthCheckOptions>(Configuration.GetSection(HealthCheckOptions.HealthCheck));
            services.Configure<GrpcOptions>(Configuration.GetSection(GrpcOptions.Grpc));

            services.AddScoped<IApplicationWatcherService, Services.ApplicationWatcherService>();

            services.AddTransient<IGrpcClientService, GrpcClientService>();
            services.AddTransient<IRegistryService, RegistryService>();
            services.AddTransient<IProcessService, ProcessService>();

            services.AddTransient<IPathWrapperService, PathWrapperService>();
            services.AddTransient<IProcessWrapperService, ProcessWrapperService>();
            services.AddTransient<IRegistryWrapperService, RegistryWrapperService>();

            services.AddHostedService<HealthCheckService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
