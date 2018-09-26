using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Manager.Messages
{
    public class ProviderHasKeysRequestMessage : BaseTransportRequestMessage
    {
        public string Id { get; set; }
    }
}
