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
    public interface IAccidentReportDialogHandler
    {
        Task<bool> AdvanceDialogAsync(IChatUpdateContext context, IAccidentReportDialogState state, CancellationToken cancellationToken);
    }

    internal sealed class AccidentReportDialogHandler : IAccidentReportDialogHandler
    {
        private readonly ILogger<AccidentReportDialogHandler> _logger;
        private readonly IAccidentReportingService _accidentReportingService;
        private readonly IBotTelemetryService _botTelemetry;
        private readonly IPhoneNumberParser _phoneNumberParser;

        public AccidentReportDialogHandler(
            ILogger<AccidentReportDialogHandler> logger,
            IAccidentReportingService accidentReportingService, 
            IBotTelemetryService botTelemetry,
            IPhoneNumberParser phoneNumberParser,
            IAccidentReportDialogMessages messages)
        {
            _logger = logger;
            _accidentReportingService = accidentReportingService;
            _botTelemetry = botTelemetry;
            _phoneNumberParser = phoneNumberParser;

            Messages = messages;
        }

        private IAccidentReportDialogMessages Messages { get; }

        /// <summary>
        /// Advances dialog with specified context and state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if dialog ended.</returns>
        public async Task<bool> AdvanceDialogAsync(IChatUpdateContext context, IAccidentReportDialogState state, CancellationToken cancellationToken)
        {
            async Task SendMessageAsync(IMessage message)
            {
                await context.SendMessageAsync(message, cancellationToken);
            }

            var update = context.Update;
            var dialogTelemetry = _botTelemetry.GetTelemetryServiceForAccidentReporting(state);

            var cancelled = CheckIfCancelled();
            if (cancelled)
            {
                dialogTelemetry.OnCancelled();

                await SendMessageAsync(Messages.Cancelled);

                return true;
            }

            try
            {
                var dialogCompleted = await HandleStepAsync();

                if (!dialogCompleted)
                {
                    state.CurrentStep++;
                    dialogTelemetry.OnNextStep();
                }
                else
                {
                    dialogTelemetry.OnCompleted();
                }

                return dialogCompleted;
            }
            catch (UnexpectedReplyTypeException)
            {
                dialogTelemetry.OnUnexpectedReply();

                return false;
            }
            catch (ReplyValidationException replyValidationException)
            {
                dialogTelemetry.OnReplyValidationFailed();

                await SendMessageAsync(replyValidationException.UserFriendlyErrorMessage);

                return false;
            }

            async Task<bool> HandleStepAsync()
            {
                switch (state.CurrentStep)
                {
                    case 0:
                        {
                            await SendMessageAsync(Messages.SpecifyAddress);
                            break;
                        }

                    case 1:
                        {
                            if (update is IMapLocation location)
                            {
                                dialogTelemetry.OnLocationSentAutomatically();

                                state.Location = location;
                            }
                            else if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 100);
                                state.Address = text;
                            }
                            else
                            {
                                throw new UnexpectedReplyTypeException();
                            }

                            await SendMessageAsync(Messages.SpecifyParticipants);
                            break;
                        }

                    case 2:
                        {
                            if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 100);
                                state.Participant = text;

                                await SendMessageAsync(Messages.AreThereVictims);
                                break;
                            }
                            
                            throw new UnexpectedReplyTypeException();
                        }

                    case 3:
                        {
                            if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 100);
                                state.Victims = text;

                                await SendMessageAsync(Messages.AskForContacts);
                                break;
                            }

                            throw new UnexpectedReplyTypeException();
                        }

                    case 4:
                        {
                            if (update is IContactMessageBotUpdate { Contact: { PhoneNumber: var phoneNumber } })
                            {
                                dialogTelemetry.OnPhoneNumberSharedAutomatically();

                                state.ReporterPhoneNumber = phoneNumber;
                            }
                            else if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 30);

                                if (!_phoneNumberParser.TryParse(text, out var parsedPhoneNumber))
                                {
                                    throw new ReplyValidationException(Messages.InvalidPhoneNumberError);
                                }

                                state.ReporterPhoneNumber = parsedPhoneNumber;
                            }
                            else
                            {
                                throw new UnexpectedReplyTypeException();
                            }

                            await SendMessageAsync(Messages.ReportSummary(state));
                            break;
                        }

                    case 5:
                        {
                            if (update is ITextMessageBotUpdate textMessage)
                            {
                                if (textMessage.Text.Trim().Equals(Messages.SubmitButton.Text, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    await ReportAccidentAsync();
                                    await SendMessageAsync(Messages.SuccessfullySent);

                                    return true;
                                }
                                else
                                {
                                    throw new ReplyValidationException(Messages.SubmitConfirmationExpectedError);
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
                                       text.Trim().Equals(Messages.CancelButton.Text, StringComparison.InvariantCultureIgnoreCase);

            async Task ReportAccidentAsync()
            {
                var report = AccidentReport.CreateFromDialogState(state);

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
                throw new ReplyValidationException(Messages.ReplyMaxLengthExceededError(maxLength));
            }
        }
    }
}