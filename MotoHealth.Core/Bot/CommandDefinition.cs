﻿using System;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot
{
    public class CommandDefinition
    {
        public CommandDefinition(string command, string? description = null)
        {
            if (!command.StartsWith("/"))
            {
                throw new ArgumentException($"{nameof(command)} should start with '/'");
            }

            Command = command;
            Description = description;
        }

        public string Command { get; }

        public string? Description { get; }

        public bool Matches(ICommandBotUpdate commandUpdate) 
            => commandUpdate.Command == Command;
    }
}