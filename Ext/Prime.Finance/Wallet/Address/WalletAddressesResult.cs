using System.Collections.Generic;
using Prime.Core;

namespace Prime.Finance
{
    public class WalletAddressesResult : ResponseModelBase
    {
        public WalletAddresses WalletAddresses { get; set; } = new WalletAddresses();

        public WalletAddressesResult()
        {
            
        }

        public WalletAddressesResult(WalletAddress address)
        {
            Add(address);
        }
        
        public void Add(WalletAddress walletAddress)
        {
            WalletAddresses.Add(walletAddress);
        }

        public bool AddRange(IEnumerable<WalletAddress> addresses)
        {
            return WalletAddresses.AddRange(addresses);
        }
    }
}