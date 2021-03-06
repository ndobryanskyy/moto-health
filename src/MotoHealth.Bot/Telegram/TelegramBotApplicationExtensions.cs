﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Bot.Middleware;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.ChatUpdateHandlers;

namespace MotoHealth.Bot.Telegram
{
    public static class TelegramBotApplicationExtensions
    {
        public static IServiceCollection AddTelegramBot(this IServiceCollection services)
        {
            services
                .AddSingleton<IBotInitializer, BotInitializer>()
                .AddSingleton<ReliableUpdateHandlingContextMiddleware>()
                .AddSingleton<BotTokenVerificationMiddleware>()
                .AddSingleton<BotUpdateInitializerMiddleware>()
                .AddTransient<ChatUpdatesFilterMiddleware>()
                .AddSingleton<ChatLockingMiddleware>()
                .AddTransient<NewChatsHandlerMiddleware>()
                .AddTransient<BannedUsersHandlerMiddleware>()
                .AddTransient(typeof(ChatUpdateHandlerMiddleware<>))
                .AddTransient<TerminatingChatHandlerMiddleware>();

            services
                .AddHostedService<BotInitializerStartupJob>();

            return services;
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
                .UseMiddleware<BannedUsersHandlerMiddleware>()
                .UseMiddleware<ChatUpdateHandlerMiddleware<AccidentReportingDialogChatUpdateHandler>>()
                .UseMiddleware<ChatUpdateHandlerMiddleware<MainCommandsChatUpdateHandler>>()
                .UseMiddleware<TerminatingChatHandlerMiddleware>()
                .Build();

            return builder
                .MapPost(Constants.Telegram.WebhookPath, pipeline)
                .WithDisplayName("Telegram Webhook");
        }
    }
}