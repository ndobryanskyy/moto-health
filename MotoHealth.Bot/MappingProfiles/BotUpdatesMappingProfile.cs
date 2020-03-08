using AutoMapper;
using MotoHealth.Bot.Telegram;
using MotoHealth.Bot.Telegram.Updates;
using MotoHealth.BotUpdates;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Bot.MappingProfiles
{
    public class BotUpdatesMappingProfile : Profile
    {
        public BotUpdatesMappingProfile()
        {
            CreateMap<BotCommand, BotCommandDtoEnum>().ReverseMap();

            CreateMap<Chat, ChatDto>()
                .ForMember(
                    x => x.IsGroup,
                    src => src.MapFrom(x => x.Type != ChatType.Private))
                .ReverseMap();

            CreateMap<TextMessageBotUpdate, TextMessageBotUpdateDto>().ReverseMap();
            CreateMap<CommandBotUpdate, CommandBotUpdateDto>().ReverseMap();
        }
    }
}