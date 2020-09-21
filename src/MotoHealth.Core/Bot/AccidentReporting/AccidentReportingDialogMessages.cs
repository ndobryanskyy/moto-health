using System.Collections.Generic;
using MotoHealth.Telegram.Extensions;
using MotoHealth.Telegram.Messages;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public interface IAccidentReportingDialogMessages
    {
        KeyboardButton CancelButton { get; }
        
        KeyboardButton SharePhoneNumberButton { get; }

        KeyboardButton ShareLocationButton { get; }

        KeyboardButton SubmitButton { get; }

        IMessage Cancelled { get; }

        IMessage SpecifyAddress { get; }

        IMessage SpecifyAddressRePrompt { get; }

        IMessage SpecifyParticipants { get; }

        IMessage SpecifyParticipantsRePrompt { get; }

        IMessage AreThereVictims { get; }

        IMessage AreThereVictimsRePrompt { get; }

        IMessage AskForContacts { get; }

        IMessage AskForContactsRePrompt { get; }

        IMessage InvalidPhoneNumberError { get; }

        IMessage ReportSummary(IAccidentReportingDialogState state);

        IMessage SubmitConfirmationRePrompt { get; }

        IMessage SuccessfullySent { get; }

        IMessage ReplyMaxLengthExceededError(int maxLength);
    }

    internal sealed class AccidentReportingDialogMessages : IAccidentReportingDialogMessages
    {
        private static readonly KeyboardButton CancelButton = new KeyboardButton("Отмена");
        private static readonly KeyboardButton SharePhoneNumberButton = KeyboardButton.WithRequestContact("Мой номер");
        private static readonly KeyboardButton ShareLocationButton = KeyboardButton.WithRequestLocation("Мое местоположение");
        private static readonly KeyboardButton SubmitButton = new KeyboardButton("Отправить");

        KeyboardButton IAccidentReportingDialogMessages.CancelButton => CancelButton;
        KeyboardButton IAccidentReportingDialogMessages.SharePhoneNumberButton => SharePhoneNumberButton;
        KeyboardButton IAccidentReportingDialogMessages.ShareLocationButton => ShareLocationButton;
        KeyboardButton IAccidentReportingDialogMessages.SubmitButton => SubmitButton;

        #region Specify Address

        private static readonly IEnumerable<IEnumerable<KeyboardButton>> SpecifyAddressReplyKeyboard = new[]
        {
            new[] { ShareLocationButton },
            new[] { CancelButton }
        };

        public IMessage SpecifyAddress { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(SpecifyAddressPrompt)
            .AddMessage(SpecifyAddressHint);

        private static readonly IMessage SpecifyAddressPrompt = MessageFactory.CreateTextMessage()
            .WithPlainText("📍 Адрес ДТП")
            .WithReplyKeyboard(SpecifyAddressReplyKeyboard);

        private static readonly IMessage SpecifyAddressHint = MessageFactory.CreateTextMessage()
            .WithHtml($"Нажмите <b>{ShareLocationButton.Text}</b>, чтобы автоматически отправить место на карте, где вы сейчас находитесь (<b>Геолокация</b> на устройстве должна быть <b>включена</b>) или напишите сообщением вручную");

        public IMessage SpecifyAddressRePrompt { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(SpecifyAddressRePromptHint);

        private static readonly IMessage SpecifyAddressRePromptHint = MessageFactory.CreateTextMessage()
            .WithHtml("Пожалуйста, отправьте адрес ДТП, используя геопозицию, или <i>напишите</i> сообщением вручную")
            .WithReplyKeyboard(SpecifyAddressReplyKeyboard);

        #endregion

        #region Specify Participants

        private static readonly IEnumerable<IEnumerable<KeyboardButton>> SpecifyParticipantsReplyKeyboard = new[]
        {
            new[] { new KeyboardButton("Мотоцикл") },
            new[] { new KeyboardButton("Мопед"), new KeyboardButton("Велосипед") },
            new[] { CancelButton }
        };

        public IMessage SpecifyParticipants { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("🛵 Участник ДТП")
            .WithReplyKeyboard(SpecifyParticipantsReplyKeyboard);

        public IMessage SpecifyParticipantsRePrompt { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(SpecifyParticipantsRePromptHint);

        private static readonly IMessage SpecifyParticipantsRePromptHint = MessageFactory.CreateTextMessage()
            .WithHtml("Пожалуйста, выберите участника ДТП или <i>напишите</i> сообщением вручную")
            .WithReplyKeyboard(SpecifyParticipantsReplyKeyboard);

        #endregion

        #region Are There Victims

        private static readonly IEnumerable<IEnumerable<KeyboardButton>> AreThereVictimsReplyKeyboard = new[]
        {
            new[] { new KeyboardButton("Да"), new KeyboardButton("Нет") },
            new[] { CancelButton }
        };

        public IMessage AreThereVictims { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("🤕 Есть пострадавшие?")
            .WithReplyKeyboard(AreThereVictimsReplyKeyboard);

        public IMessage AreThereVictimsRePrompt { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(AreThereVictimsRePromptHint);

        private static readonly IMessage AreThereVictimsRePromptHint = MessageFactory.CreateTextMessage()
            .WithHtml("Пожалуйста, выберите есть ли пострадавшие в ДТП или <i>напишите</i> сообщением вручную")
            .WithReplyKeyboard(AreThereVictimsReplyKeyboard);

        #endregion

        #region Ask For Contacts

        private static readonly IEnumerable<IEnumerable<KeyboardButton>> AskForContactsReplyKeyboard = new[]
        {
            new[] { SharePhoneNumberButton },
            new[] { CancelButton }
        };

        public IMessage AskForContacts { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(AskForContactsPrompt)
            .AddMessage(AskForContactsHint);

        private static readonly IMessage AskForContactsPrompt = MessageFactory.CreateTextMessage()
            .WithPlainText("📞 Номер для обратной связи")
            .WithReplyKeyboard(AskForContactsReplyKeyboard);

        private static readonly IMessage AskForContactsHint = MessageFactory.CreateTextMessage()
            .WithHtml($"Нажмите <b>{SharePhoneNumberButton.Text}</b>, чтобы автоматически отправить свой номер телефона, либо напишите другой вручную");

        public IMessage AskForContactsRePrompt { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(AskForContactsRePromptHint);

        private static readonly IMessage AskForContactsRePromptHint = MessageFactory.CreateTextMessage()
            .WithHtml("Пожалуйста, отправьте свой номер телефона автоматически или <i>напишите</i> сообщением вручную")
            .WithReplyKeyboard(AskForContactsReplyKeyboard);

        public IMessage InvalidPhoneNumberError { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(InvalidPhoneNumberErrorHint);

        private static readonly IMessage InvalidPhoneNumberErrorHint = MessageFactory.CreateTextMessage()
            .WithHtml("Пожалуйста, напишите телефон как <i>0671234567</i> или <i>380501234567</i>");

        #endregion

        #region Report Summary

        private static readonly ReplyKeyboard ReportSummaryReplyKeyboard = new ReplyKeyboard
        {
            new[] { SubmitButton },
            new[] { CancelButton }
        };

        public IMessage ReportSummary(IAccidentReportingDialogState state) => MessageFactory.CreateTextMessage()
            .WithHtml(
                "🚨 Сообщение о ДТП\n\n" +
                $"• <b>Адрес:</b> {state.Address?.HtmlEscaped() ?? "Геопозиция"}\n" +
                $"• <b>Участник:</b> {state.Participant.HtmlEscaped()}\n" +
                $"• <b>Пострадавшие:</b> {state.Victims.HtmlEscaped()}\n" +
                $"• <b>Телефон:</b> {state.ReporterPhoneNumber.HtmlEscaped()}\n\n" +
                "<i>Отправить?</i>")
            .WithReplyKeyboard(ReportSummaryReplyKeyboard);

        public IMessage SubmitConfirmationRePrompt { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(SubmitConfirmationRePromptHint);

        private static readonly IMessage SubmitConfirmationRePromptHint = MessageFactory.CreateTextMessage()
            .WithHtml($"Пожалуйста, нажмите <b>{SubmitButton.Text}</b>, чтобы сообщить о ДТП или <b>{CancelButton.Text}</b>, чтобы завершить без отправки")
            .WithReplyKeyboard(ReportSummaryReplyKeyboard);

        #endregion

        #region Successfully Sent

        public IMessage SuccessfullySent { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(SuccessfullySentConfirmation)
            .AddMessage(BeforeArrivalHint);

        private static readonly IMessage SuccessfullySentConfirmation = MessageFactory.CreateTextMessage()
            .WithPlainText("✅ Успешно отправлено, ожидайте звонка на указанный вами номер")
            .WithClearedReplyKeyboard();

        private static readonly IMessage BeforeArrivalHint = MessageFactory.CreateTextMessage()
            .WithHtml(
                "🙏 До приезда оператора, вы можете <i>помочь:</i>\n\n" +
                "• <b>Пострадавшие:</b> не перемещать, постараться обездвижить, вызвать скорую помощь\n" +
                "• <b>Место ДТП:</b> обезопасить и оградить, не перемещать транспортные средства, постараться сохранить следы ДТП (следы протектора и т.п.)\n" +
                "• <b>Свидетели:</b> записать ФИО и номер телефона\n" +
                "• <b>Скорая:</b> записать номер бригады и в какую больницу везут пострадавшего\n\n" +
                "<i>Спасибо!</i>");

        #endregion

        public IMessage Cancelled { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("⛔ Отменено")
            .WithClearedReplyKeyboard();

        public IMessage ReplyMaxLengthExceededError(int maxLength) => MessageFactory.CreateTextMessage()
            .WithHtml($"😮 Максимальная длина ответа - <b>{maxLength}</b> символов, пожалуйста, сократите сообщение");
    }
}