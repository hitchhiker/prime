using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Settings
{
    public class TimeKindUpdatedRequestMessage : BaseTransportRequestMessage
    {
        public bool IsUtcTime { get; set; }
    }
}
