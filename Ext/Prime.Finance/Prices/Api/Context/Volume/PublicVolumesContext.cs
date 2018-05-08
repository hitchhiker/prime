using Prime.Core;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Finance
{
    public class PublicVolumesContext : PublicContextBase
    {
        public PublicVolumesContext(ILogger logger = null) : base(logger) { }

        public PublicVolumesContext(IList<AssetPair> pairs, ILogger logger = null) : base(pairs) { }
    }
}