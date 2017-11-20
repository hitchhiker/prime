using Prime.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Common
{
    public class PublicVolumesContext : PublicContextBase
    {
        public PublicVolumesContext(IList<AssetPair> pairs, ILogger logger = null) : base(pairs) { }
    }
}