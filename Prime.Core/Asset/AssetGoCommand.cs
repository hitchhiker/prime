using System;
using System.Linq;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetGoCommand : CommandContent, IEquatable<AssetGoCommand>
    {
        public AssetGoCommand() { }

        public AssetGoCommand(Asset asset)
        {
            Asset = asset;
            Title = 
            Command = "ASSET " + asset.ShortCode;
        }

        [Bson]
        public Asset Asset { get; private set; }
        
        public override CommandBase Parse(string scmd)
        {
            if (!scmd.StartsWith("a"))
                return null;

            var commands = GetParts(scmd);
            if (commands.Count != 2)
                return null;

            var asi = AssetInfos.Get();
            var m = asi.Items.FirstOrDefault(x => string.Equals(x.FullName, commands[1], StringComparison.OrdinalIgnoreCase))?.Asset;
            if (m != null)
                return new AssetGoCommand(m);

            m = asi.Items.FirstOrDefault(x => string.Equals(x.Asset.ShortCode, commands[1], StringComparison.OrdinalIgnoreCase))?.Asset;
            if (m != null)
                return new AssetGoCommand(m);

            return null;
        }

        public override string DefaultTitle => "Asset " + Asset.ShortCode;

        public override bool Equals(CommandContent other)
        {
            return Equals(other as AssetGoCommand);
        }

        public bool Equals(AssetGoCommand other)
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
            return Equals((AssetGoCommand) obj);
        }

        public override int GetHashCode()
        {
            return (Asset != null ? Asset.GetHashCode() : 0);
        }
    }
}