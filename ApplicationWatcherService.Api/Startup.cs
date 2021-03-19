using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ApplicationWatcherService.Api.Interfaces;
using ApplicationWatcherService.Api.Models.Options;
using ApplicationWatcherService.Api.Services.HostedServices;
using ApplicationWatcherService.Utils.Interfaces;
using ApplicationWatcherService.Utils.Services;

namespace ApplicationWatcherService.Api
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

            services.AddTransient<ISvGrpcClientService, SvGrpcClientService>();
            services.AddTransient<IRegistryService, RegistryService>();
            services.AddTransient<IProcessService, ProcessService>();

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
