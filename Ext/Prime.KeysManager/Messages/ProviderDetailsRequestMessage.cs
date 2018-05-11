using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.KeysManager.Messages
{
    public class ProviderDetailsRequestMessage : BaseTransportRequestMessage
    {
        public string Id { get; set; }
    }
}
