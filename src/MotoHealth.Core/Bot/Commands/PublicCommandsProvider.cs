using System.Linq;
using System.Reflection;
using Telegram.Bot.Types;

namespace MotoHealth.Core.Bot.Commands
{
    public interface IPublicCommandsProvider
    {
        BotCommand[] Commands { get; }
    }

    internal sealed class PublicCommandsProvider : IPublicCommandsProvider
    {
        private BotCommand[]? _commands;

        public BotCommand[] Commands
        {
            get
            {
                _commands ??= GetCommandsFromCoreAssembly();
                
                return _commands;
            }
        }

        private BotCommand[] GetCommandsFromCoreAssembly() =>
            GetType().Assembly
                .DefinedTypes
                .Select(x => x.GetCustomAttribute<PublicBotCommandAttribute>())
                .Where(x => x != null)
                .Select(x =>
                {
                    var (name, description) = x;

                    return new BotCommand
                    {
                        Command = name,
                        Description = description
                    };
                })
                .ToArray();
    }
}