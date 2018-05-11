using System.Collections.Generic;
using Prime.Core;
using Prime.KeysManager.Core.Models;

namespace Prime.KeysManager.Messages
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