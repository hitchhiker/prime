using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Settings
{
    public class UpdateTimeKindRequestMessage : BaseTransportRequestMessage
    {
        public bool IsUtc { get; set; }
    }
}
