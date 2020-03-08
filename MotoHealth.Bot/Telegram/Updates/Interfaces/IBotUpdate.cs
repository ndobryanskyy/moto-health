using AutoMapper;

namespace MotoHealth.Bot.Telegram.Updates
{
    public interface IBotUpdate
    {
        int UpdateId { get; }

        IChatContext Chat { get; }

        byte[] Serialize(IMapper mapper);
    }
}