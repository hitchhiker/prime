using System.Linq;
using LiteDB;
using Prime.Core;

namespace Prime.Core
{
    public class CoverageMapModel : CoverageMapBase, IModelBase
    {
        public override OhlcData Include(TimeRange rangeAttempted, OhlcData data, bool acceptLiveRange = false)
        {
            base.Include(rangeAttempted, data, acceptLiveRange);

            this.SavePublic();

            return data;
        }
    }
}