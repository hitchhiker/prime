using System;

namespace Prime.Common
{
    public class VolumeSource : ApiSource
    {
        private VolumeSource() { }

        public VolumeSource(INetworkProvider provider, Type interfaceType) : base(provider, interfaceType)
        {
        }

        [Bson]
        public bool IsBulk { get; set; }

        public override string ToString()
        {
            return base.ToString() + " Bulk:{IsBulk}";
        }
    }
}