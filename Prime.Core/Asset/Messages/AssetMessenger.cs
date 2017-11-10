using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core.AssetMessages
{
    public class AssetMessenger : IStartupMessenger
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;

        public AssetMessenger()
        {
            _messenger.RegisterAsync<AssetAllRequestMessage>(this, AllRequestMessage);
            _messenger.RegisterAsync<AssetNetworkRequestMessage>(this, AssetNetworkRequestMessage);
        }

        private async void AllRequestMessage(AssetAllRequestMessage m)
        {
            var assets = await AssetProvider.I.GetAllAsync(true).ConfigureAwait(false);
            var currentAsssets = assets.Where(x => !Equals(x, Common.Asset.None)).OrderBy(x => x.ShortCode).ToList();
            _messenger.SendAsync(new AssetAllResponseMessage(currentAsssets, m.RequesterToken));
        }

        private async void AssetNetworkRequestMessage(AssetNetworkRequestMessage m)
        {
            var assets = await AssetProvider.I.GetAssetsAsync(m.Network).ConfigureAwait(false);

            if (assets?.Any() == true)
                _messenger.SendAsync(new AssetNetworkResponseMessage(m.Network, assets));
        }
    }
}