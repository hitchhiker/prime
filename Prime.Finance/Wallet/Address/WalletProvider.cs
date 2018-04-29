using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Finance.Wallet
{
    public class WalletProvider
    {
        private readonly UserContext _context;

        public WalletProvider(UserContext context)
        {
            _context = context;
        }

        private UniqueList<WalletAddress> Addresses => _context.Finance().UserSettings.Addresses;

        public async Task<IReadOnlyList<WalletAddress>> GenerateNewAddressAsync(Network network, Asset asset)
        {
            var service = network.GetProviders<IDepositProvider>().Where(x=>x.CanGenerateDepositAddress).FirstProvider();
            if (service == null)
                return null;

            var r = await ApiCoordinator.GetDepositAddressesAsync(service, new WalletAddressAssetContext(asset, _context)).ConfigureAwait(false);
            if (r.IsNull)
                return null;

            var ads = r.Response.WalletAddresses;
            var usr = Addresses;

            foreach (var a in usr)
            {
                if (ads.Contains(a))
                    ads.Remove(a);
            }

            if (!ads.Any())
                return null;

            usr.AddRange(ads);
            _context.Finance().UserSettings.Save(_context);

            return ads;
        }

        public IReadOnlyList<WalletAddress> GetAll()
        {
            return Addresses;
        }
    }
}