using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;

namespace Prime.Finance.AssetMessages
{
    public class AssetMessenger : IStartupMessenger
    {
        public IMessenger M { get; private set; }

        public void Start(PrimeContext context)
        {
            M = context.M;
            M.RegisterAsync<AssetAllRequestMessage>(this, AllRequestMessage);
            M.RegisterAsync<AssetNetworkRequestMessage>(this, AssetNetworkRequestMessage);
        }

        public void Stop()
        {
            M?.UnregisterAsync(this);
        }

        private async void AllRequestMessage(AssetAllRequestMessage m)
        {
            var assets = await AssetProvider.I.GetAllAsync(true).ConfigureAwait(false);
            var currentAsssets = assets.Where(x => !Equals(x, Asset.None)).OrderBy(x => x.ShortCode).ToList();
            M.SendAsync(new AssetAllResponseMessage(currentAsssets, m.RequesterToken));
        }

        private async void AssetNetworkRequestMessage(AssetNetworkRequestMessage m)
        {
            var assets = await AssetProvider.I.GetAssetsAsync(m.Network).ConfigureAwait(false);

            if (assets?.Any() == true)
                M.SendAsync(new AssetNetworkResponseMessage(m.Network, assets));
        }
    }
}