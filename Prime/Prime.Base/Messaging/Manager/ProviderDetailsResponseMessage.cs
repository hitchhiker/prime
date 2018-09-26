using Prime.Base.Messaging.Manager.Models;
using Prime.Core;

namespace Prime.Manager.Messages
{
    public class ProviderDetailsResponseMessage : BaseTransportResponseMessage
    {
        public ProviderDetailsResponseMessage(ProviderDetailsRequestMessage request, NetworkDetailsModel response) : base(request)
        {
            Response = response;
        }

        public NetworkDetailsModel Response { get; set; }
    }
}