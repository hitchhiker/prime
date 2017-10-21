using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common.Messages
{
    public class CommandAcceptedMessage
    {
        public readonly CommandBase Command;

        public CommandAcceptedMessage(CommandBase command)
        {
            Command = command;
        }
    }
}
