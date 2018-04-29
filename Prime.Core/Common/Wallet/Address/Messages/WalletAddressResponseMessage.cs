namespace Prime.Core
{
    public class WalletAddressResponseMessage
    {
        public readonly WalletAddress Address;

        public WalletAddressResponseMessage(WalletAddress address)
        {
            Address = address;
        }
    }
}