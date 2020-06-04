﻿using System;
using System.Linq;
using AutoMapper;
using MotoHealth.Core.Bot.Commands;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Chat = Telegram.Bot.Types.Chat;

namespace MotoHealth.Core.Telegram
{
    public sealed class TelegramMappingProfile : Profile
    {
        public TelegramMappingProfile()
        {
            CreateMap<CommandDefinition, BotCommand>();

            CreateMap<Contact, TelegramContact>();
            CreateMap<User, TelegramUser>();
            CreateMap<Chat, TelegramGroup>();

            CreateMessageBotUpdateMap<TextMessageBotUpdate>()
                .ForMember(
                    x => x.Text,
                    opts => opts.MapFrom(x => x.Message.Text)
                );

            CreateMessageBotUpdateMap<CommandMessageBotUpdate>()
                .ForMember(
                    x => x.Command,
                    opts => opts.MapFrom(x => x.Message.EntityValues.First().Split("@", StringSplitOptions.RemoveEmptyEntries).First())
                )
                .ForMember(
                    x => x.Arguments,
                    opts => opts.MapFrom(x => x.Message.Text.Substring(x.Message.Entities.First().Length).Trim())
                );

            CreateMessageBotUpdateMap<ContactMessageBotUpdate>()
                .ForMember(
                    x => x.Contact,
                    opts => opts.MapFrom(x => x.Message.Contact)
                );

            CreateMessageBotUpdateMap<LocationMessageBotUpdate>()
                .ForMember(
                    x => x.Latitude,
                    opts => opts.MapFrom(x => x.Message.Location.Latitude)
                )
                .ForMember(
                    x => x.Longitude,
                    opts => opts.MapFrom(x => x.Message.Location.Longitude)
                );

            CreateMessageBotUpdateMap<NotMappedMessageBotUpdate>()
                .ForMember(
                    x => x.OriginalUpdate,
                    opts => opts.MapFrom(x => x)
                )
                .ForMember(
                    x => x.OriginalMessage,
                    opts => opts.MapFrom(x => x.Message)
                );

            CreateMap<Update, NotMappedBotUpdate>()
                .ForMember(
                    x => x.UpdateId,
                    opts => opts.MapFrom(x => x.Id)
                )
                .ForMember(
                    x => x.OriginalUpdate,
                    opts => opts.MapFrom(x => x)
                );
        }

        private IMappingExpression<Update, TMessageBotUpdate> CreateMessageBotUpdateMap<TMessageBotUpdate>()
            where TMessageBotUpdate : MessageBotUpdateBase
            => CreateMap<Update, TMessageBotUpdate>()
                .ForMember(
                    x => x.UpdateId,
                    opts => opts.MapFrom(x => x.Id)
                )
                .ForMember(
                    x => x.MessageId,
                    opts => opts.MapFrom(x => x.Message.MessageId)
                )
                .ForMember(
                    x => x.Sender,
                    opts => opts.MapFrom(x => x.Message.From)
                )
                .ForMember(
                    x => x.Chat,
                    opts => opts.MapFrom((source, destination, _, context) =>
                    {
                        var chat = source.Message.Chat;

                        ITelegramGroup? group = null;

                        if (chat.Type != ChatType.Private)
                        {
                            group = context.Mapper.Map<TelegramGroup>(chat);
                        }

                        return new TelegramChat
                        {
                            Id = chat.Id,
                            Type = chat.Type,
                            Group = group
                        };
                    })
                );
    }
}