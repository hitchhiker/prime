using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Base.Messaging.Manager
{
    public class TimeKindUpdatedRequestMessage : BaseTransportRequestMessage
    {
        public bool IsUtcTime { get; set; }
    }
}
