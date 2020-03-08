using AutoMapper;
using MotoHealth.BotUpdates;

namespace MotoHealth.Bot.Telegram.Updates
{
    internal sealed class CommandBotUpdate : MessageBotUpdate, ICommandBotUpdate
    {
        public CommandBotUpdate(
            int updateId,
            IChatContext chat, 
            BotCommand command, 
            string[] arguments)
            : base(updateId, chat)
        {
            Command = command;
            Arguments = arguments;
        }
        
        public BotCommand Command { get; }

        public string[] Arguments { get; }

        protected override void SetActualBotUpdate(BotUpdateDto updateDto, IMapper mapper)
        {
            updateDto.Command = mapper.Map<CommandBotUpdateDto>(this);
        }
    }
}