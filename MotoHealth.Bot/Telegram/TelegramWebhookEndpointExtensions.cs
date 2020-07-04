﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Bot.Middleware;
using MotoHealth.Core.Bot.ChatUpdateHandlers;

namespace MotoHealth.Bot.Telegram
{
    public static class TelegramWebhookEndpointExtensions
    {
        public static IServiceCollection AddTelegramBot(this IServiceCollection services)
        {
            return services
                .AddTransient<ReliableUpdateHandlingContextMiddleware>()
                .AddSingleton<BotTokenVerificationMiddleware>()
                .AddSingleton<BotUpdateInitializerMiddleware>()
                .AddTransient<ChatUpdatesFilterMiddleware>()
                .AddSingleton<ChatLockingMiddleware>()
                .AddTransient<NewChatsHandlerMiddleware>()
                .AddTransient(typeof(ChatUpdateHandlerMiddleware<>))
                .AddTransient<TerminatingChatHandlerMiddleware>();
        }

        public static IEndpointConventionBuilder MapTelegramWebhook(this IEndpointRouteBuilder builder)
        {
            var pipeline = builder.CreateApplicationBuilder()
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

            return builder
                .MapPost(Constants.Telegram.WebhookPath, pipeline)
                .WithDisplayName("Telegram Webhook");
        }
    }
}