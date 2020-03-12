using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MotoHealth.Bot.Messages;
using MotoHealth.Bot.ServiceBus;
using MotoHealth.Bot.Telegram;

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

            services.AddAutoMapper(GetType().Assembly);

            services.Configure<TelegramOptions>(_configuration.GetSection(Constants.Telegram.ConfigurationSectionName));

            services.AddSingleton<IQueueClientsFactory, QueueClientsFactory>();

            services.AddSingleton<ITelegramBotClientFactory, TelegramBotClientFactory>();

            services.AddSingleton<IBotUpdateResolver, BotUpdateResolver>();

            services.AddSingleton<IBotUpdateSerializer, BotUpdateSerializer>();

            services.AddSingleton<ITelegramUpdatesQueue, TelegramUpdatesQueue>();

            services.AddHostedService<UpdatesQueueHandlerBackgroundService>();
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
