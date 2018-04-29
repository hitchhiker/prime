using Prime.Core.Messages;

namespace Prime.Finance
{
    public class AssetAllRequestMessage : RequestorTokenMessageBase
    {
        public AssetAllRequestMessage(string requesterToken) : base(requesterToken)
        {
        }
    }
}
