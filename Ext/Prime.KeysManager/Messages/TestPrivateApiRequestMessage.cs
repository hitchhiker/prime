using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.KeysManager.Messages
{
    public class TestPrivateApiRequestMessage : BaseTransportRequestMessage
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Extra { get; set; }
    }
}
