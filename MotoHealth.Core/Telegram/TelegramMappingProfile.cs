using AutoMapper;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Telegram
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