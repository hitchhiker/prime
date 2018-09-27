﻿using Prime.Base.DStore;

namespace Prime.Core
{
    public class PinContentResponse : BaseTransportResponseMessage
    {
        private PinContentResponse() { }

        public PinContentResponse(PinContentRequest request, string protocol) : base(request)
        {
            LocalHash = request.LocalHash;
            Protocol = protocol;
        }

        public string LocalHash { get; set; }

        public string Protocol { get; set; }

        public ContentUri ContentUri { get; set; }

        public bool Success { get; set; }
    }
}