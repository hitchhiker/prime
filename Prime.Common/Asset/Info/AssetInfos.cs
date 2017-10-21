using System;
using System.Linq;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetInfos : ModelBase
    {
        private AssetInfos() { }

        [Bson]
        public UniqueList<AssetInfo> Items { get; set; } = new UniqueList<AssetInfo>();

        [Bson]
        public DateTime UtcLastUpdated { get; set; }

        public void Refresh()
        {
            if (UtcLastUpdated.IsWithinTheLast(TimeSpan.FromDays(7)))
                return;

            DoRefresh();

            UtcLastUpdated = DateTime.UtcNow;
            this.SavePublic();
        }

        private void DoRefresh()
        {
            var cc = Networks.I.CoinListProviders.FirstProvider();
            var r = ApiCoordinator.GetCoinInfo(cc);
            if (r.IsNull)
                return;

            Items.Clear();
            Items.AddRange(r.Response);
            Items.Add(new AssetInfo() {Asset = Assets.I.GetRaw("EUR"), FullName = "Euro", SortOrder = 1});
            Items.Add(new AssetInfo() {Asset = Assets.I.GetRaw("USD"), FullName = "U.S. dollar", SortOrder = 1});
        }

        public static AssetInfos Get()
        {
            var c = PublicContext.I;
            var ai = c.As<AssetInfos>().FirstOrDefault() ?? new AssetInfos();
            ai.Refresh();
            return ai;
        }

        public AssetInfo FirstOrDefault(Asset asset)
        {
            var ai = Items.FirstOrDefault(x => x.Asset == asset);
            if (ai != null)
                return ai;

            Items.Add(ai = new AssetInfo() {Asset = asset, Remote = new Remote(), FullName = asset.ShortCode, SortOrder = 100});
            this.SavePublic();
            return ai;
        }
    }
}