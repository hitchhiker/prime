using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class VolumeFeatures : FeaturesBase<VolumeSingleFeatures, VolumeBulkFeatures>
    {
        public VolumeFeatures() { }

        public VolumeFeatures(bool single, bool bulk) : base(single, bulk) { }

    }
}
