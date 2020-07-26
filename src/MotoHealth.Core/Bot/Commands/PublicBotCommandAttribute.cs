using System;

namespace MotoHealth.Core.Bot.Commands
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PublicBotCommandAttribute : Attribute
    {
        public PublicBotCommandAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public CommandName Name { get; }

        public string Description { get; }

        public void Deconstruct(out CommandName name, out string description)
        {
            name = Name;
            description = Description;
        }
    }
}