using System;
using System.Collections.Generic;
using AutoMapper;
using MotoHealth.BotUpdates;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Infrastructure.UpdatesQueue
{
    public sealed class UpdatesQueueMappingProfile : Profile
    {
        private readonly Dictionary<BotUpdateDto.PayloadOneofCase, Type> _payloadCaseToUpdateTypeMap = new Dictionary<BotUpdateDto.PayloadOneofCase, Type>();

        public UpdatesQueueMappingProfile()
        {
            CreateMap<BotCommand, BotCommandDtoEnum>().ReverseMap();

            CreateMap<ITelegramUser, TelegramUserDto>();
            CreateMap<TelegramUserDto, TelegramUser>();

            CreateMap<ITelegramGroup, TelegramGroupDto>();
            CreateMap<TelegramGroupDto, TelegramGroup>();

            CreateMap<ITelegramChat, TelegramChatDto>();
            CreateMap<TelegramChatDto, TelegramChat>();

            CreateMap<ITelegramContact, TelegramContactDto>();
            CreateMap<TelegramContactDto, TelegramContact>();

            CreateUpdateMap<TextMessageBotUpdate, TextMessageBotUpdateDto>(BotUpdateDto.PayloadOneofCase.TextMessage);
            CreateUpdateMap<CommandBotUpdate, CommandBotUpdateDto>(BotUpdateDto.PayloadOneofCase.Command);
            CreateUpdateMap<ContactMessageBotUpdate, ContactMessageBotUpdateDto>(BotUpdateDto.PayloadOneofCase.ContactMessage);

            CreateUpdateDtoReverseMapping();
        }

        private void CreateUpdateMap<TUpdate, TUpdateDto>(BotUpdateDto.PayloadOneofCase payloadCase)
            where TUpdate: class, IBotUpdate
        {
            var caseAdded = _payloadCaseToUpdateTypeMap.TryAdd(payloadCase, typeof(TUpdate));

            if (!caseAdded)
            {
                throw new InvalidOperationException($"Duplicate mapping case for {nameof(BotUpdateDto)} - {payloadCase}");
            }

            CreateMap<TUpdate, TUpdateDto>()
                .ReverseMap();

            CreateMap<TUpdate, BotUpdateDto>()
                .ForMember(
                    payloadCase.ToString(),
                    opt => opt.MapFrom(x => x))
                .ForAllOtherMembers(x => x.Ignore());
        }

        private void CreateUpdateDtoReverseMapping()
        {
            CreateMap<BotUpdateDto, IBotUpdate>()
                .ConvertUsing((dto, update, context) =>
                {
                    if (_payloadCaseToUpdateTypeMap.TryGetValue(dto.PayloadCase, out var destinationType))
                    {
                        // TODO cache reflection
                        var payloadProperty = typeof(BotUpdateDto).GetProperty(dto.PayloadCase.ToString()) ?? throw new InvalidOperationException();
                        var payloadValue = payloadProperty.GetValue(dto);

                        return (IBotUpdate)context.Mapper.Map(payloadValue, payloadProperty.PropertyType, destinationType);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException($"No mapping exists for payload case {dto.PayloadCase}");
                    }
                });
        }
    }
}