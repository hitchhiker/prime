﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Finance
{
    public class VolumeFeatures : FeaturesBase<VolumeSingleFeatures, VolumeBulkFeatures>
    {
        public VolumeFeatures() { }

        public VolumeFeatures(bool single, bool bulk) : base(single, bulk) { }
    }
}
