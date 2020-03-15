using System;
using System.Linq.Expressions;
using AutoMapper;
using MotoHealth.BotUpdates;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Infrastructure.UpdatesQueue
{
    public sealed class UpdatesQueueMappingProfile : Profile
    {
        public UpdatesQueueMappingProfile()
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