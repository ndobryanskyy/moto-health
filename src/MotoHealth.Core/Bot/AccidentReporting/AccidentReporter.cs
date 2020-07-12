namespace MotoHealth.Core.Bot.AccidentReporting
{
    public sealed class AccidentReporter
    {
        public AccidentReporter(int telegramUserId, string phoneNumber)
        {
            TelegramUserId = telegramUserId;
            PhoneNumber = phoneNumber;
        }

        public int TelegramUserId { get; }

        public string PhoneNumber { get; }
    }
}