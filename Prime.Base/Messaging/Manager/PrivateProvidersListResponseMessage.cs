using System.Collections.Generic;
using Prime.Base.Messaging.Manager.Models;
using Prime.Core;

namespace Prime.Manager.Messages
{
    public class PrivateProvidersListResponseMessage : BaseTransportResponseMessage
    {
        public PrivateProvidersListResponseMessage(PrivateProvidersListRequestMessage request, List<NetworkModel> response) : base(request)
        {
            Response = response;
        }

        public List<NetworkModel> Response { get; set; }
    }
}