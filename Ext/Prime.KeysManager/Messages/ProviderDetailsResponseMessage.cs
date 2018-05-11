using Prime.Core;
using Prime.KeysManager.Core.Models;

namespace Prime.KeysManager.Messages
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