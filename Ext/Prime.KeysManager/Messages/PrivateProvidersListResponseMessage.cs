using System.Collections.Generic;
using Prime.Core;
using Prime.KeysManager.Core.Models;

namespace Prime.KeysManager.Messages
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