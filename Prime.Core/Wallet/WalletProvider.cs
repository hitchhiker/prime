using System;
using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Core
{
    public class WalletProvider
    {
        private readonly UserContext _context;

        public WalletProvider(UserContext context)
        {
            _context = context;
        }

        private UniqueList<WalletAddress> Addresses => _context.UserSettings.Addresses;

        public void AddAddress(IWalletService service, Asset asset, Action after)
        {
            var wa = service.FetchDepositAddresses(new WalletAddressAssetContext(asset, true, _context));
            if (wa == null)
                return;

            Addresses.AddRange(wa);
            _context.UserSettings.Save(_context);
            after();
        }

        public IReadOnlyList<WalletAddress> GetAll()
        {
            return Addresses;
        }
    }
}