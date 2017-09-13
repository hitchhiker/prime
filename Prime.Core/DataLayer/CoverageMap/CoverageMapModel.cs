using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class CoverageMapModel : CoverageMapBase, IModelBase
    {
        public override OhclData Include(TimeRange rangeAttempted, OhclData data, bool acceptLiveRange = false)
        {
            base.Include(rangeAttempted, data, acceptLiveRange);

            this.SavePublic();

            return data;
        }
    }
}