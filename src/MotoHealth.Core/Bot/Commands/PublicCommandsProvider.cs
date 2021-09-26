using System;
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
        private readonly Lazy<BotCommand[]> _commands = new Lazy<BotCommand[]>(GetCommandsFromCoreAssembly);

        public BotCommand[] Commands => _commands.Value;

        private static BotCommand[] GetCommandsFromCoreAssembly() =>
            typeof(PublicCommandsProvider).Assembly
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