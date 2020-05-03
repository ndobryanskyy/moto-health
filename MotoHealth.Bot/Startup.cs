using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MotoHealth.Core;
using MotoHealth.Infrastructure;

namespace MotoHealth.Bot
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddAutoMapper(new []
            {
                CoreApplicationPartMarker.Assembly,
                InfrastructureApplicationPartMarker.Assembly,
                GetType().Assembly
            });

            services.AddApp(_configuration);

            services.AddCore(new CoreOptionsConfigurator
            {
                ConfigureTelegram = telegramOptions =>
                {
                    _configuration.Bind(Constants.Telegram.ConfigurationSectionName, telegramOptions);
                }
            });

            services.AddInfrastructure(new InfrastructureOptionsConfigurator
            {
                ConfigureChatStorage = chatsStorageOptions =>
                {
                    chatsStorageOptions.StorageAccountConnectionString = _configuration.GetConnectionString(Constants.ChatsStorage.ConnectionStringName);
                },
                ConfigureAccidentsQueue = accidentsQueueOptions =>
                {
                    accidentsQueueOptions.ConnectionString = _configuration.GetConnectionString(Constants.AccidentsQueue.ConnectionStringName);
                }
            });
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
                endpoints.Map("/", context =>
                {
                    var telemetry = context.Features.Get<RequestTelemetry>();
                    telemetry.Name = "Always On Ping";

                    context.Response.StatusCode = StatusCodes.Status200OK;
                    
                    return Task.CompletedTask;
                });

                endpoints.MapControllers();
            });
        }
    }
}
