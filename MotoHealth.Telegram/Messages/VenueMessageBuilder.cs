using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace MotoHealth.Telegram.Messages
{
    public sealed class VenueMessageBuilder : IMessage
    {
        private double? _latitude;
        private double? _longitude;
        private string _title = string.Empty;
        private string _address = string.Empty;

        public VenueMessageBuilder WithLocation(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;

            return this;
        }

        public VenueMessageBuilder WithTitle(string title)
        {
            _title = title;

            return this;
        }

        public VenueMessageBuilder WithAddress(string address)
        {
            _address = address;

            return this;
        }

        public async Task SendAsync(
            ChatId chatId, 
            ITelegramClient client, 
            CancellationToken cancellationToken)
        {
            if (!_latitude.HasValue || !_longitude.HasValue)
            {
                // TODO throw domain level exception
                throw new InvalidOperationException("Location must be set");
            }

            var request = new SendVenueRequest(
                chatId, 
                (float)_latitude.Value, (float)_longitude.Value, 
                _title, _address
            );

            await client.SendVenueMessageAsync(request, cancellationToken);
        }
    }
}