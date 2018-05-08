using Prime.Core;

namespace Prime.Finance
{
    public class WalletAllResponseMessage
    {
        public readonly UniqueList<WalletAddress> Addresses;

        public WalletAllResponseMessage(UniqueList<WalletAddress> addresses)
        {
            Addresses = addresses;
        }
    }
}