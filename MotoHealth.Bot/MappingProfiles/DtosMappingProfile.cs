using System;
using System.Linq.Expressions;
using AutoMapper;
using MotoHealth.Bot.Telegram;
using MotoHealth.Bot.Telegram.Updates;
using MotoHealth.BotUpdates;

namespace MotoHealth.Bot.MappingProfiles
{
    public sealed class DtosMappingProfile : Profile
    {
        public DtosMappingProfile()
        {
            CreateMap<BotCommand, BotCommandDtoEnum>().ReverseMap();

            CreateMap<IChatContext, ChatContextDto>();

            CreateMap<ChatContextDto, ChatContext>();
            CreateMap<ChatContextDto, IChatContext>()
                .As<ChatContext>();

            CreateUpdateMap<ITextMessageBotUpdate, TextMessageBotUpdate, TextMessageBotUpdateDto>(updateDto => updateDto.TextMessage);
            CreateUpdateMap<ICommandBotUpdate, CommandBotUpdate, CommandBotUpdateDto>(updateDto => updateDto.Command);
        }

        private void CreateUpdateMap<TUpdateInterface, TUpdate, TUpdateDto>(Expression<Func<BotUpdateDto, TUpdateDto>> payloadCasePropertyExpression)
            where TUpdate: class, TUpdateInterface
        {
            CreateMap<TUpdateInterface, TUpdateDto>();

            CreateMap<TUpdateInterface, BotUpdateDto>()
                .ForMember(
                    payloadCasePropertyExpression,
                    opt => opt.MapFrom(x => x))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<TUpdateDto, TUpdate>();
        }
    }
}