using AutoMapper;
using MotoHealth.Core.Bot.Updates;
using Telegram.Bot.Types;

namespace MotoHealth.Core.Telegram
{
    public sealed class TelegramMappingProfile : Profile
    {
        public TelegramMappingProfile()
        {
            CreateMap<Contact, TelegramContact>();
            CreateMap<User, TelegramUser>();
        }
    }
}