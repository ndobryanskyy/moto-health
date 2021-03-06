using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MotoHealth.Bot.AppInsights;
using MotoHealth.Bot.Authorization;
using MotoHealth.Bot.Extensions;
using MotoHealth.Bot.HealthChecks;
using MotoHealth.Bot.Telegram;
using MotoHealth.Bot.Telegram.HealthChecks;
using MotoHealth.Core;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure;
using MotoHealth.Telegram;

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
            services.AddRouting();
            
            services
                .AddHealthChecks()
                .AddTelegram();

            services.AddAutoMapper(new []
            {
                CoreApplicationPartMarker.Assembly,
                InfrastructureApplicationPartMarker.Assembly,
                GetType().Assembly
            });

            services
                .AddOptions<AuthorizationSecretsOptions>()
                .Bind(_configuration.GetSection(Constants.Authorization.SecretsConfigurationSectionName));

            services
                .AddOptions<TelegramOptions>()
                .Bind(_configuration.GetSection(Constants.Telegram.ConfigurationSectionName));

            services
                .AddOptions<TelegramClientOptions>()
                .Bind(_configuration
                        .GetSection(Constants.Telegram.ConfigurationSectionName)
                        .GetSection(nameof(TelegramOptions.Client)));

            services
                .AddTelegramBot()
                .AddAppInsights(_configuration)
                .AddSingleton<IAuthorizationSecretsService, AuthorizationSecretsService>();

            services.AddCore();

            services.AddInfrastructure(options =>
            {
                _configuration.Bind(Constants.AzureStorage.ConfigurationSectionName, options.AzureStorage);
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
                endpoints.MapHealthChecks();
                endpoints.MapAlwaysOnPing();
                endpoints.MapTelegramWebhook();
            });
        }
    }
}
