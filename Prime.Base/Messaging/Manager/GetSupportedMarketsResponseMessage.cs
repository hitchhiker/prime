using System.Collections.Generic;
using Prime.Base.Messaging.Manager.Models;
using Prime.Core;

namespace Prime.Manager.Messages
{
    public class GetSupportedMarketsResponseMessage : BaseTransportResponseMessage
    {
        public GetSupportedMarketsResponseMessage(GetSupportedMarketsRequestMessage request) : base(request)
        {
            
        }
        
        public IEnumerable<MarketModel> Markets { get; set; }
    }
}