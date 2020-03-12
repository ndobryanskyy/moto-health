using AutoMapper;
using MotoHealth.Bot.Extensions;
using MotoHealth.Bot.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Bot.MappingProfiles
{
    public sealed class TelegramMappingProfile : Profile
    {
        public TelegramMappingProfile()
        {
            CreateMap<Chat, ChatContext>()
                .ForCtorParam(
                    nameof(ChatContext.IsGroup).ToCamelCase(),
                    x => x.MapFrom(src => src.Type != ChatType.Private));
        }
    }
}