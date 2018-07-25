using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.SocketServer.Transport.Cleaner
{
    public class PollingCleanerEventArgs<TClientId> : EventArgs
    {
        public TClientId ClientId { get; set; }
    }
}
