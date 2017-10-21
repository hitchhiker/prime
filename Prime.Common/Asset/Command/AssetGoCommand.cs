using System;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
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

        public bool Equals(AssetGoCommand other)
        {
            return base.Equals(this);
        }
    }
}