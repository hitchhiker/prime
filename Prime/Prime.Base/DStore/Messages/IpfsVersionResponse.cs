using System.Collections.Generic;

namespace Prime.Core
{
    public class IpfsVersionResponse : BaseTransportResponseMessage
    {
        public string Version { get; set; }
        public Dictionary<string,string> Items { get; set; }
    }
}