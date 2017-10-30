using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetMessenger : IStartupMessenger
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;

        public AssetMessenger()
        {
            /*
            "USD".ToAssetRaw();
            "EUR".ToAssetRaw();
            "BTC".ToAssetRaw();
            "XRP".ToAssetRaw();
            "ETH".ToAssetRaw();*/

            _messenger.RegisterAsync<AssetAllRequestMessage>(this, AllRequestMessage);
            _messenger.RegisterAsync<AssetNetworkRequestMessage>(this, AssetNetworkRequestMessage);
        }

        private async void AllRequestMessage(AssetAllRequestMessage m)
        {
            var assets = await Assets.I.GetAllPrivateAsync();
            var currentAsssets = assets.Where(x => !Equals(x, Asset.None)).OrderBy(x => x.ShortCode).ToList();
            _messenger.SendAsync(new AssetAllResponseMessage(currentAsssets, m.RequesterToken));
        }

        private async void AssetNetworkRequestMessage(AssetNetworkRequestMessage m)
        {
            var assets = await AssetProvider.I.GetAssetsAsync(m.Network);

            if (assets?.Any() == true)
                _messenger.SendAsync(new AssetNetworkResponseMessage(m.Network, assets));
        }
    }
}