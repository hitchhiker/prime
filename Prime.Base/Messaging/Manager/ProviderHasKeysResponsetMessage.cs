using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Manager.Messages
{
    public class ProviderHasKeysResponsetMessage : BooleanResponseMessage
    {
        public ProviderHasKeysResponsetMessage(ProviderHasKeysRequestMessage request, bool hasKeys) : base(request, hasKeys)
        {
            
        }
    }
}
