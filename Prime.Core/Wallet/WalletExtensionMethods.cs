using System;
using System.Linq;

namespace Prime.Core
{
    public static class WalletExtensionMethods
    {
        public static WalletAddress GetLatestDepositAddress(this UserContext context, IWalletService provider, Asset asset)
        {
            var wd = context.Data(provider);
            var w = wd.GetLatest(asset);
            if (w != null && w.IsFresh())
                return w;

            var ws = provider.FetchDepositAddresses(new WalletAddressAssetContext(asset, false, context));

            if (ws?.Count==0)
                ws = provider.FetchDepositAddresses(new WalletAddressAssetContext(asset, true, context));

            if (ws?.Count == 0)
                return null;

            wd.AddRange(ws);
            wd.Save(context);
            return wd.GetLatest(asset);
        }
    }
}