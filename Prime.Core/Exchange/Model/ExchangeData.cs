using System;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class ExchangeData : ModelBase, IKeepFresh, IOnNewInstance
    {
        public void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> provider)
        {
            KeepFresh(context, provider as IExchangeProvider, true);
        }

        public static object Lock = new Object();

        [Bson]
        public AssetPairs AssetPairs { get; set; } = new AssetPairs();

        [Bson]
        public UniqueList<Asset> Assets { get; set; } = new UniqueList<Asset>();

        public void KeepFresh(IDataContext context, IExchangeProvider exchange, bool ignoreFreshState = false)
        {
            if (IsFresh(ignoreFreshState))
                return;

            lock (Lock)
            {
                if (IsFresh(ignoreFreshState))
                    return;

                var t = exchange.GetAssetPairs(new NetworkProviderContext());

                t.Wait();
                AssetPairs = t.Result;
                AssetPairs.UtcLastUpdated = DateTime.UtcNow;

                Assets = AssetPairs.Select(x => x.Asset1).Union(AssetPairs.Select(x => x.Asset2)).OrderBy(x=>x.ShortCode).ToUniqueList();

                this.Save(context);
            }
        }


        private bool IsFresh(bool ignoreFreshState)
        {
            return !ignoreFreshState && AssetPairs.UtcLastUpdated.IsWithinTheLast(TimeSpan.FromHours(12));
        }
    }
}