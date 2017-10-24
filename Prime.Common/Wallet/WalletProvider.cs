using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Common
{
    public class WalletProvider
    {
        private readonly UserContext _context;

        public WalletProvider(UserContext context)
        {
            _context = context;
        }

        private UniqueList<WalletAddress> Addresses => _context.UserSettings.Addresses;

        public async Task<IReadOnlyList<WalletAddress>> GenerateNewAddress(Network network, Asset asset)
        {
            var service = network.WalletProviders.FirstProvider();
            if (!service.CanGenerateDepositAddress)
                return null;

            var r = await ApiCoordinator.GetDepositAddressesAsync(service, new WalletAddressAssetContext(asset, _context));
            if (r.IsNull)
                return null;

            var ads = r.Response;
            var usr = Addresses;

            foreach (var a in usr)
            {
                if (ads.Contains(a))
                    ads.Remove(a);
            }

            if (!ads.Any())
                return null;

            usr.AddRange(ads);
            _context.UserSettings.Save(_context);

            return ads;
        }

        public IReadOnlyList<WalletAddress> GetAll()
        {
            return Addresses;
        }
    }
}