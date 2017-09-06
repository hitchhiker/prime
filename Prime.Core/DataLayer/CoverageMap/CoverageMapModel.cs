using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class CoverageMapModel : CoverageMapBase, IModelBase
    {
        public override OhclData Include(TimeRange rangeAttempted, OhclData data)
        {
            base.Include(rangeAttempted, data);

            this.SavePublic();

            return data;
        }
    }
}