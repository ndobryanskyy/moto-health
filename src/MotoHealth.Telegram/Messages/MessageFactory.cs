namespace MotoHealth.Telegram.Messages
{
    public static class MessageFactory
    {
        public static TextMessageBuilder CreateTextMessage()
            => new TextMessageBuilder();

        public static VenueMessageBuilder CreateVenueMessage()
            => new VenueMessageBuilder();

        public static CompositeMessageBuilder CreateCompositeMessage()
            => new CompositeMessageBuilder();
    }
}