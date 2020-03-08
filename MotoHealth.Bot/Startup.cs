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
        private const string TelegramSectionName = "Telegram";

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMvcCore();

            services.Configure<TelegramOptions>(_configuration.GetSection(TelegramSectionName));

            services.AddSingleton<QueueClientsProvider>();

            services.AddSingleton<IMessagesQueueSenderClientProvider>(
                container => container.GetRequiredService<QueueClientsProvider>()
            );

            services.AddSingleton<IMessagesQueueReceiverClientProvider>(
                container => container.GetRequiredService<QueueClientsProvider>()
            );

            services.AddSingleton<IBotClientProvider, BotClientProvider>();

            services.AddSingleton<IBotUpdateResolver, BotUpdateResolver>();

            services.AddSingleton<ITelegramUpdatesQueue, TelegramUpdatesQueue>();

            services.AddHostedService<MessagesHandlerBackgroundService>();
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
