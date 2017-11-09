using System;
using System.Linq;

namespace Prime.Common
{
    public static class WalletExtensionMethods
    {
        /// <summary>
        /// This is broken, needs a review -> get all address, or just the latest?
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static WalletAddress GetLatestDepositAddress(this UserContext context, IDepositProvider provider, Asset asset)
        {
            /* var wd = context.Data(provider);
             var w = wd.GetLatest(asset);
             if (w != null && w.IsFresh())
                 return w;

             WalletAddress address = null;

             var r = ApiCoordinator.GetDepositAddresses(provider, new WalletAddressAssetContext(asset, context));
             if (!r.IsNull && r.Response.Count != 0)
                 address = r.Response.OrderByDescending(x=>x.UtcCreated).FirstOrDefault();

             if (address == null)
             {
                 var r2 = ApiCoordinator.GetDepositAddresses(provider, new WalletAddressAssetContext(asset, context));
                 if (r2.IsNull || r2.Response.Count == 0)
                     return null;

                 address = r2.Response.OrderByDescending(x => x.UtcCreated).FirstOrDefault();
             }

             wd.Add(address);
             wd.Save(context);
             return wd.GetLatest(asset);*/
            return null;
        }
    }
}