using System;

namespace MotoHealth.Telegram.Exceptions
{
    public enum TelegramApiError
    {
        Unexpected,
        BadRequest,
        Forbidden,
        InternalServerError,
    }

    public sealed class TelegramApiException : Exception
    {
        public TelegramApiError Type { get; }

        public string ErrorDescription { get; }

        public TelegramApiException(
            TelegramApiError type,
            string errorDescription) : base(errorDescription)
        {
            Type = type;
            ErrorDescription = errorDescription;
        }
    }
}