using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common
{
    public class AssetInfo : IEquatable<AssetInfo>
    {
        [Bson]
        public Asset Asset { get; set; }

        [Bson]
        public Remote Remote { get; set; }

        [Bson]
        public string Url { get; set; }

        [Bson]
        public string ImageUrl { get; set; }

        [Bson]
        public string BackgroundColour => AssetCustomColours.CustomColor(Asset);

        [Bson]
        public string ExternalInfoUrl { get; set; }

        [Bson]
        public string FullName { get; set; }

        [Bson]
        public string Algorithm { get; set; }

        [Bson]
        public string ProofType { get; set; }

        [Bson]
        public bool FullyPremined { get; set; }

        [Bson]
        public decimal TotalCoinSupply { get; set; }

        [Bson]
        public decimal PreMinedValue { get; set; }

        [Bson]
        public decimal TotalCoinsFreeFloat { get; set; }

        [Bson]
        public int SortOrder { get; set; }

        /*
        private BitmapSource _logoBitmap;
        public BitmapSource LogoBitmap => _logoBitmap ?? (_logoBitmap = ImageUrl.ToImageFromUri(PublicContext.I));
        */

        public bool Equals(AssetInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Asset, other.Asset);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssetInfo) obj);
        }

        public override int GetHashCode()
        {
            return (Asset != null ? Asset.GetHashCode() : 0);
        }
    }
}
