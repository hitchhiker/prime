using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class VolumeFeaturesItemBase
    {
        public bool CanVolumeBase { get; set; }
        public bool CanVolumeQuote { get; set; }
        public bool CanVolume => CanVolumeBase || CanVolumeQuote;
    }
}
