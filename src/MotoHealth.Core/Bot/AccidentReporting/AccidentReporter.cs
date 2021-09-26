namespace MotoHealth.Core.Bot.AccidentReporting
{
    public sealed class AccidentReporter
    {
        public AccidentReporter(long telegramUserId, string phoneNumber)
        {
            TelegramUserId = telegramUserId;
            PhoneNumber = phoneNumber;
        }

        public long TelegramUserId { get; }

        public string PhoneNumber { get; }
    }
}