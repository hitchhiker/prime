﻿using System;
using Prime.Core;

namespace Prime.KeysManager.Messages
{
    public class GenerateGuidResponseMessage : BaseTransportResponseMessage
    {
        public readonly Guid Guid;
        public GenerateGuidResponseMessage() { }

        public GenerateGuidResponseMessage(BaseTransportRequestMessage request, Guid guid) : base(request)
        {
            Guid = guid;
        }
    }
}