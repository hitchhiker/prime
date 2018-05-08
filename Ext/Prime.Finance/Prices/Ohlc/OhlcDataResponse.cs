using System.Collections;
using System.Collections.Generic;
using Prime.Core;

namespace Prime.Finance
{
    public class OhlcDataResponse : ResponseModelBase, IEnumerable<OhlcEntry>
    {
        public OhlcData OhlcData { get; private set; }

        public OhlcDataResponse(TimeResolution resolution)
        {
            OhlcData = new OhlcData(resolution);
        }

        public void Add(OhlcEntry entry)
        {
            OhlcData.Add(entry);
        }

        public void Reverse()
        {
            OhlcData.Reverse();
        }

        public IEnumerator<OhlcEntry> GetEnumerator()
        {
            return OhlcData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this).GetEnumerator();
        }
    }
}