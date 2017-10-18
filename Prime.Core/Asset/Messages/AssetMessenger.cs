using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Core
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
        }

        private void AllRequestMessage(AssetAllRequestMessage m)
        {
            var currentAsssets = Core.Assets.I.Cached().Where(x => !Equals(x, Asset.None)).OrderBy(x => x.ShortCode).ToList();
            _messenger.SendAsync(new AssetAllResponseMessage(currentAsssets, m.RequesterToken));
        }
    }
}