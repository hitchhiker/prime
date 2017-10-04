using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task AddAddressAsync(IWalletService service, Asset asset)
        {
            var r = await ApiCoordinator.GetDepositAddressesAsync(service, new WalletAddressAssetContext(asset, true, _context));
            if (r.IsNull)
                return;

            var wa = r.Response;
            if (wa == null)
                return;

            Addresses.AddRange(wa);
            _context.UserSettings.Save(_context);
        }

        public IReadOnlyList<WalletAddress> GetAll()
        {
            return Addresses;
        }
    }
}