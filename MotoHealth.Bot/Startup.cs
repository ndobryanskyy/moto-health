using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Authorization;
using MotoHealth.Bot.Extensions;
using MotoHealth.Bot.Middleware;
using MotoHealth.Core;
using MotoHealth.Core.Bot.ChatUpdateHandlers;
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

            services.Configure<AuthorizationSecretsOptions>(options =>
            {
                _configuration.Bind(Constants.Authorization.SecretsConfigurationSectionName, options);
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
                ConfigureEventGrid = eventGridOptions =>
                {
                    _configuration.Bind(Constants.AzureEventGrid.ConfigurationSectionName, eventGridOptions);
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
                endpoints.Map("/", OnAlwaysOnPingAsync);
                endpoints.MapPost("/updates", CreateTelegramPipeline(endpoints.CreateApplicationBuilder()));
            });
        }

        private static Task OnAlwaysOnPingAsync(HttpContext context)
        {
            var logger = context.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("AlwaysOn");

            var requestTelemetry = context.GetRequestTelemetry();
            requestTelemetry.Context.Operation.SyntheticSource = Constants.ApplicationInsights.AlwaysOnPingSyntheticSource;

            context.Response.StatusCode = StatusCodes.Status200OK;

            logger.LogDebug("Always On Ping Received");

            return Task.CompletedTask;
        }

        private static RequestDelegate CreateTelegramPipeline(IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<BotTokenVerificationMiddleware>()
                .UseMiddleware<ReliableUpdateHandlingContextMiddleware>()
                .UseMiddleware<BotUpdateInitializerMiddleware>()
                .UseMiddleware<ChatUpdatesFilterMiddleware>()
                .UseMiddleware<ChatLockingMiddleware>()
                .UseMiddleware<NewChatsHandlerMiddleware>()
                .UseMiddleware<ChatUpdateHandlerMiddleware<BannedUsersChatUpdateHandler>>()
                .UseMiddleware<ChatUpdateHandlerMiddleware<AdminCommandsChatUpdateHandler>>()
                .UseMiddleware<ChatUpdateHandlerMiddleware<AccidentReportingDialogChatUpdateHandler>>()
                .UseMiddleware<ChatUpdateHandlerMiddleware<MainChatUpdateHandler>>()
                .UseMiddleware<TerminatingChatHandlerMiddleware>()
                .Build();
        }
    }
}
