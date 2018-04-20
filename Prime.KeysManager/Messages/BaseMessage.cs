using System;

namespace Prime.KeysManager.Messages
{
    public class BaseMessage
    {
        public string Type { get; set; }
        public Guid SenderGuid { get; set; }
    }
}