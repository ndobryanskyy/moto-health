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

            CreateMap<TextMessageBotUpdate, TextMessageBotUpdateDto>();
            CreateMap<TextMessageBotUpdate, BotUpdateDto>()
                .ForMember(
                    x => x.TextMessage,
                    opt => opt.MapFrom(x => x));

            CreateMap<TextMessageBotUpdateDto, TextMessageBotUpdate>();

            CreateMap<ICommandBotUpdate, CommandBotUpdateDto>();
            CreateMap<CommandBotUpdateDto, CommandBotUpdate>();
        }
    }
}