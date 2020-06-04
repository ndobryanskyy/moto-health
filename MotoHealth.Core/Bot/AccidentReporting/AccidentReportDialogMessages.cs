using MotoHealth.Telegram.Extensions;
using MotoHealth.Telegram.Messages;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public interface IAccidentReportDialogMessages
    {
        KeyboardButton CancelButton { get; }
        
        KeyboardButton SharePhoneNumberButton { get; }

        KeyboardButton ShareLocationButton { get; }

        KeyboardButton SubmitButton { get; }

        IMessage Cancelled { get; }

        IMessage SpecifyAddress { get; }

        IMessage SpecifyParticipants { get; }

        IMessage AreThereVictims { get; }

        IMessage AskForContacts { get; }

        IMessage InvalidPhoneNumberError { get; }

        IMessage ReportSummary(IAccidentReportDialogState state);

        IMessage SubmitConfirmationExpectedError { get; }

        IMessage SuccessfullySent { get; }

        IMessage ReplyMaxLengthExceededError(int maxLength);
    }

    internal sealed class AccidentReportDialogMessages : IAccidentReportDialogMessages
    {
        KeyboardButton IAccidentReportDialogMessages.CancelButton => CancelButton;
        KeyboardButton IAccidentReportDialogMessages.SharePhoneNumberButton => SharePhoneNumberButton;
        KeyboardButton IAccidentReportDialogMessages.ShareLocationButton => ShareLocationButton;
        KeyboardButton IAccidentReportDialogMessages.SubmitButton => SubmitButton;

        public IMessage Cancelled { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("⛔ Отменено")
            .WithClearedReplyKeyboard();

        public IMessage SpecifyAddress { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(SpecifyAddressPrompt)
            .AddMessage(SpecifyAddressHint);
        
        public IMessage SpecifyParticipants { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("🛵 Участник ДТП")
            .WithReplyKeyboard(new[]
            {
                new [] { new KeyboardButton("Мотоцикл") },
                new [] { new KeyboardButton("Мопед"), new KeyboardButton("Велосипед") },
                new [] { CancelButton }
            });

        public IMessage AreThereVictims { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("🤕 Есть пострадавшие?")
            .WithReplyKeyboard(new[]
            {
                new [] { new KeyboardButton("Да"), new KeyboardButton("Нет") },
                new [] { CancelButton }
            });

        public IMessage AskForContacts { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(AskForContactsPrompt)
            .AddMessage(AskForContactsHint);

        public IMessage InvalidPhoneNumberError { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(InvalidPhoneNumberErrorHint);
        
        public IMessage ReportSummary(IAccidentReportDialogState state) => MessageFactory.CreateTextMessage()
            .WithHtml(
                "🚨 Сообщение о ДТП\n\n" +
                $"• <b>Адрес:</b> {state.Address?.HtmlEscaped() ?? "Геопозиция"}\n" +
                $"• <b>Участник:</b> {state.Participant.HtmlEscaped()}\n"+
                $"• <b>Пострадавшие:</b> {state.Victims.HtmlEscaped()}\n" +
                $"• <b>Телефон:</b> {state.ReporterPhoneNumber.HtmlEscaped()}")
            .WithReplyKeyboard(ReportSummaryKeyboard);

        public IMessage SubmitConfirmationExpectedError { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(SubmitConfirmationExpectedErrorHint);
        
        public IMessage SuccessfullySent { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("✅ Успешно отправлено, ожидайте звонка на указанный вами номер")
            .WithClearedReplyKeyboard();

        public IMessage ReplyMaxLengthExceededError(int maxLength) => MessageFactory.CreateTextMessage()
            .WithHtml($"😮 Максимальная длина ответа - <b>{maxLength}</b> символов, пожалуйста, сократите сообщение");

        private static readonly KeyboardButton CancelButton = new KeyboardButton("Отмена");
        private static readonly KeyboardButton SharePhoneNumberButton = KeyboardButton.WithRequestContact("Мой номер");
        private static readonly KeyboardButton ShareLocationButton = KeyboardButton.WithRequestLocation("Мое местоположение");
        private static readonly KeyboardButton SubmitButton = new KeyboardButton("Отправить");

        private static readonly IMessage SpecifyAddressPrompt = MessageFactory.CreateTextMessage()
            .WithPlainText("📍 Адрес ДТП");

        private static readonly IMessage SpecifyAddressHint = MessageFactory.CreateTextMessage()
            .WithHtml($"Нажмите <b>{ShareLocationButton.Text}</b>, чтобы автоматически отправить место на карте, где сейчас находитесь (<b>Геолокация</b> на устройстве должна быть включена), либо напишите вручную")
            .WithReplyKeyboard(new[]
            {
                new [] { ShareLocationButton },
                new [] { CancelButton }
            });

        private static readonly IMessage AskForContactsPrompt = MessageFactory.CreateTextMessage()
            .WithPlainText("📞 Номер для обратной связи");

        private static readonly IMessage AskForContactsHint = MessageFactory.CreateTextMessage()
            .WithHtml($"Нажмите <b>{SharePhoneNumberButton.Text}</b>, чтобы автоматически отправить свой номер телефона, либо напишите другой вручную")
            .WithReplyKeyboard(new[]
            {
                new [] { SharePhoneNumberButton },
                new [] { CancelButton }
            });

        private static readonly IMessage InvalidPhoneNumberErrorHint = MessageFactory.CreateTextMessage()
            .WithHtml("Попробуйте написать телефон как <i>0671234567</i> или <i>380501234567</i>");

        private static readonly ReplyKeyboard ReportSummaryKeyboard = new ReplyKeyboard
        {
            new[] { SubmitButton },
            new[] { CancelButton }
        };

        private static readonly IMessage SubmitConfirmationExpectedErrorHint = MessageFactory.CreateTextMessage()
            .WithHtml($"Нажмите <b>{SubmitButton.Text}</b>, чтобы сообщить о ДТП или <b>{CancelButton.Text}</b>, чтобы завершить без отправки")
            .WithReplyKeyboard(ReportSummaryKeyboard);
    }
}