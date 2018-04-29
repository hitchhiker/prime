using Prime.Core;

namespace Prime.Core
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