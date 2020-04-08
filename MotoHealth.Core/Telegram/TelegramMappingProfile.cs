using AutoMapper;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Updates;
using Telegram.Bot.Types;

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
        }
    }
}