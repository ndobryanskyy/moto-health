using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting.Exceptions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public interface IAccidentReportingDialogHandler
    {
        Task StartDialogAsync(IChatUpdateContext context, CancellationToken cancellationToken);

        Task AdvanceDialogAsync(IChatUpdateContext context, CancellationToken cancellationToken);
    }

    internal sealed class AccidentReportingDialogHandler : IAccidentReportingDialogHandler
    {
        private readonly ILogger<AccidentReportingDialogHandler> _logger;
        private readonly IBotTelemetryService _botTelemetry;
        private readonly IAccidentReportingDialogMessages _messages;
        private readonly IPhoneNumberParser _phoneNumberParser;
        private readonly IAccidentReportingService _accidentReportingService;

        public AccidentReportingDialogHandler(
            ILogger<AccidentReportingDialogHandler> logger,
            IBotTelemetryService botTelemetry,
            IAccidentReportingDialogMessages messages,
            IPhoneNumberParser phoneNumberParser,
            IAccidentReportingService accidentReportingService)
        {
            _logger = logger;
            _botTelemetry = botTelemetry;
            _messages = messages;
            _phoneNumberParser = phoneNumberParser;
            _accidentReportingService = accidentReportingService;
        }

        public async Task StartDialogAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            var state = await context.GetStagingStateAsync(cancellationToken);
            var dialogState = state.AccidentReportDialog;

            if (dialogState != null)
            {
                throw new InvalidOperationException();
            }

            dialogState = state.StartAccidentReportingDialog(1);

            await context.SendMessageAsync(_messages.SpecifyAddress, cancellationToken);

            var dialogTelemetry = _botTelemetry.GetTelemetryServiceForAccidentReporting(dialogState);
            dialogTelemetry.OnStarted();
        }

        public async Task AdvanceDialogAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            async Task SendMessageAsync(IMessage message)
            {
                await context.SendMessageAsync(message, cancellationToken);
            }

            var update = context.Update;
            var state = await context.GetStagingStateAsync(cancellationToken);
            var dialogState = state.AccidentReportDialog ?? throw new InvalidOperationException();
            var dialogTelemetry = _botTelemetry.GetTelemetryServiceForAccidentReporting(dialogState);

            var cancelled = CheckIfCancelled();
            if (cancelled)
            {
                await SendMessageAsync(_messages.Cancelled);
                state.CompleteAccidentReportingDialog();

                dialogTelemetry.OnCancelled();
                return;
            }

            try
            {
                var dialogCompleted = await HandleStepAsync();

                if (dialogCompleted)
                {
                    dialogTelemetry.OnCompleted();
                    state.CompleteAccidentReportingDialog();
                }
                else
                {
                    dialogState.CurrentStep++;
                    dialogTelemetry.OnNextStep();
                }
            }
            catch (UnexpectedReplyTypeException)
            {
                dialogTelemetry.OnUnexpectedReply();
            }
            catch (ReplyValidationException replyValidationException)
            {
                dialogTelemetry.OnReplyValidationFailed();

                await SendMessageAsync(replyValidationException.UserFriendlyErrorMessage);
            }

            async Task<bool> HandleStepAsync()
            {
                switch (dialogState.CurrentStep)
                {
                    case 1:
                        {
                            if (update is IMapLocation location)
                            {
                                dialogTelemetry.OnLocationSentAutomatically();

                                dialogState.Location = location;
                            }
                            else if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 100);
                                dialogState.Address = text;
                            }
                            else
                            {
                                throw new UnexpectedReplyTypeException();
                            }

                            await SendMessageAsync(_messages.SpecifyParticipants);
                            break;
                        }

                    case 2:
                        {
                            if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 100);
                                dialogState.Participant = text;

                                await SendMessageAsync(_messages.AreThereVictims);
                                break;
                            }
                            
                            throw new UnexpectedReplyTypeException();
                        }

                    case 3:
                        {
                            if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 100);
                                dialogState.Victims = text;

                                await SendMessageAsync(_messages.AskForContacts);
                                break;
                            }

                            throw new UnexpectedReplyTypeException();
                        }

                    case 4:
                        {
                            if (update is IContactMessageBotUpdate { Contact: { PhoneNumber: var phoneNumber } })
                            {
                                dialogTelemetry.OnPhoneNumberSharedAutomatically();

                                dialogState.ReporterPhoneNumber = phoneNumber;
                            }
                            else if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 30);

                                if (!_phoneNumberParser.TryParse(text, out var parsedPhoneNumber))
                                {
                                    throw new ReplyValidationException(_messages.InvalidPhoneNumberError);
                                }

                                dialogState.ReporterPhoneNumber = parsedPhoneNumber;
                            }
                            else
                            {
                                throw new UnexpectedReplyTypeException();
                            }

                            await SendMessageAsync(_messages.ReportSummary(dialogState));
                            break;
                        }

                    case 5:
                        {
                            if (update is ITextMessageBotUpdate textMessage)
                            {
                                if (textMessage.Text.Trim().Equals(_messages.SubmitButton.Text, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    await ReportAccidentAsync();
                                    await SendMessageAsync(_messages.SuccessfullySent);

                                    return true;
                                }
                                else
                                {
                                    throw new ReplyValidationException(_messages.SubmitConfirmationExpectedError);
                                }
                            }

                            throw new UnexpectedReplyTypeException();
                        }


                    default:
                        _logger.LogError("Got unexpected step number");
                        return true;
                }

                return false;
            } 

            bool CheckIfCancelled() => update is ITextMessageBotUpdate { Text: var text } &&
                                       text.Trim().Equals(_messages.CancelButton.Text, StringComparison.InvariantCultureIgnoreCase);

            async Task ReportAccidentAsync()
            {
                var report = AccidentReport.CreateFromDialogState(dialogState);

                report.ReporterTelegramUserId = update.Sender.Id;
                report.ReportedAtUtc = DateTime.UtcNow;

                await _accidentReportingService.ReportAccidentAsync(report, cancellationToken);
            }
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local Assertion method
        private void EnsureMaxLengthNotExceeded(string text, int maxLength)
        {
            if (text.Length > maxLength)
            {
                throw new ReplyValidationException(_messages.ReplyMaxLengthExceededError(maxLength));
            }
        }
    }
}