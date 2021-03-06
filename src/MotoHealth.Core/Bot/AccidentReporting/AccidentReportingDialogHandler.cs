﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public AccidentReportingDialogHandler(
            ILogger<AccidentReportingDialogHandler> logger,
            IBotTelemetryService botTelemetry,
            IAccidentReportingDialogMessages messages,
            IMapper mapper,
            IPhoneNumberParser phoneNumberParser,
            IAccidentReportingService accidentReportingService)
        {
            _logger = logger;
            _botTelemetry = botTelemetry;
            _messages = messages;
            _mapper = mapper;
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
            catch (AccidentReportingDialogReplyException dialogException)
            {
                if (dialogException is AccidentReportingDialogReplyValidationException)
                {
                    dialogTelemetry.OnReplyValidationFailed();
                }
                else
                {
                    dialogTelemetry.OnUnexpectedReply();
                }

                await SendMessageAsync(dialogException.UserFriendlyErrorMessage);
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
                                throw new AccidentReportingDialogReplyException(_messages.SpecifyAddressRePrompt);
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
                            
                            throw new AccidentReportingDialogReplyException(_messages.SpecifyParticipantsRePrompt);
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

                            throw new AccidentReportingDialogReplyException(_messages.AreThereVictimsRePrompt);
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
                                    throw new AccidentReportingDialogReplyValidationException(_messages.InvalidPhoneNumberError);
                                }

                                dialogState.ReporterPhoneNumber = parsedPhoneNumber;
                            }
                            else
                            {
                                throw new AccidentReportingDialogReplyException(_messages.AskForContactsRePrompt);
                            }

                            await SendMessageAsync(_messages.ReportSummary(dialogState));
                            break;
                        }

                    case 5:
                        {
                            if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                var trimmed = text.Trim();

                                if (trimmed.Equals(_messages.SubmitButton.Text, StringComparison.InvariantCultureIgnoreCase) ||
                                    trimmed.Equals("да", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    await ReportAccidentAsync();
                                    await SendMessageAsync(_messages.SuccessfullySent);

                                    return true;
                                }
                                else
                                {
                                    throw new AccidentReportingDialogReplyValidationException(_messages.SubmitConfirmationRePrompt);
                                }
                            }

                            throw new AccidentReportingDialogReplyException(_messages.SubmitConfirmationRePrompt);
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
                var details = _mapper.Map<AccidentDetails>(dialogState);
                var reporter = new AccidentReporter(update.Sender.Id, dialogState.ReporterPhoneNumber);
                var report = new AccidentReport(dialogState.ReportId, DateTime.UtcNow, reporter, details);

                await _accidentReportingService.ReportAccidentAsync(report, cancellationToken);
            }
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local Assertion method
        private void EnsureMaxLengthNotExceeded(string text, int maxLength)
        {
            if (text.Length > maxLength)
            {
                throw new AccidentReportingDialogReplyValidationException(_messages.ReplyMaxLengthExceededError(maxLength));
            }
        }
    }
}