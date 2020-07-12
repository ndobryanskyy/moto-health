namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IHasSender
    {
        ITelegramUser Sender { get; }
    }
}