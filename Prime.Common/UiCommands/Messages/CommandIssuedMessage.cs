using LiteDB;

namespace Prime.Common.Messages
{
    public class CommandIssuedMessage
    {
        public readonly string StringCommand;
        public readonly ObjectId ContainerId;

        public CommandIssuedMessage(ObjectId containerId, string command)
        {
            ContainerId = containerId;
            StringCommand = command;
        }
    }
}