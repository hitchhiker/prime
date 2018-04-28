﻿using System.Linq;
using LiteDB;
using Prime.Common;

namespace Prime.Common
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