using Prime.Utility;

namespace Prime.Common
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