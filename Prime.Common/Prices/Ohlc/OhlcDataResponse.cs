namespace Prime.Common
{
    public class OhlcDataResponse : ResponseModelBase
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
    }
}