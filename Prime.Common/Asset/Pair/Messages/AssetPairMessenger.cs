using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPairMessenger : IStartupMessenger
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;

        public AssetPairMessenger()
        {
            _messenger.RegisterAsync<AssetPairNetworkRequestMessage>(this, AssetPairNetworkRequestMessage);
        }

        private async void AssetPairNetworkRequestMessage(AssetPairNetworkRequestMessage m)
        {
            var pairs = await AssetPairProvider.I.GetPairsAsync(m.Network);
            if (pairs == null)
                return;

            _messenger.SendAsync(new AssetPairNetworkResponseMessage(m.Network, pairs));
        }
    }
}