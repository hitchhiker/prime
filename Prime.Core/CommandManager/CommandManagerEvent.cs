using System;

namespace Prime.Core
{
    public class CommandManagerEvent : EventArgs
    {
        public CommandManagerEvent(CommandBase command)
        {
            Command = command;
        }

        public readonly CommandBase Command;
    }
}