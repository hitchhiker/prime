using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public abstract class FeaturesBase<TSingleFeatures, TBulkFeatures> where TSingleFeatures : class, new() where TBulkFeatures : class, new()
    {
        protected FeaturesBase()
        {
            
        }

        protected FeaturesBase(bool single, bool bulk)
        {
            Single = single ? new TSingleFeatures() : null;
            Bulk = bulk ? new TBulkFeatures() : null;
        }

        public bool HasBulk => Bulk != null;
        public bool HasSingle => Single != null;

        public TSingleFeatures Single { get; set; }
        public TBulkFeatures Bulk { get; set; }
    }
}
