using System.Collections.Generic;
using Prime.Base.Messaging.Manager.Models;
using Prime.Core;

namespace Prime.Manager.Messages
{
    public class ProvidersListResponseMessage : BaseTransportResponseMessage
    {
        public ProvidersListResponseMessage(ProvidersListRequestMessage request, List<NetworkModel> response) : base(request)
        {
            Response = response;
        }

        public List<NetworkModel> Response { get; set; }
    }
}