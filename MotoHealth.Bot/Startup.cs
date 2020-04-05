using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Console;
using MotoHealth.Bot.Telegram;
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
            services.Configure<ConsoleLoggerOptions>(options =>
            {
                options.TimestampFormat = "[HH:mm:ss.fff] ";
            });

            services.AddControllers().AddNewtonsoftJson();

            services.AddAutoMapper(new []
            {
                CoreApplicationPartMarker.Assembly,
                InfrastructureApplicationPartMarker.Assembly,
                GetType().Assembly
            });

            services.AddCore(new CoreOptionsConfigurator
            {
                ConfigureTelegram = telegramOptions =>
                {
                    _configuration.Bind(Constants.Telegram.ConfigurationSectionName, telegramOptions);
                }
            });

            services.AddInfrastructure(new InfrastructureOptionsConfigurator
            {
                ConfigureUpdatesQueue = updatesQueueOptions =>
                {
                    updatesQueueOptions.ConnectionString = _configuration.GetConnectionString(Constants.UpdatesQueue.ConnectionStringName);
                },
                ConfigureChatStorage = chatsStorageOptions =>
                {
                    chatsStorageOptions.StorageAccountConnectionString = _configuration.GetConnectionString(Constants.ChatsStorage.ConnectionStringName);
                }
            });

            services.AddTransient<IBotUpdateResolver, BotUpdateResolver>();
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
        }
    }
}
