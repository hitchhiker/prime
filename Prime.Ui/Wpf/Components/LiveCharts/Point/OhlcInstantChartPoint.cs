using NodaTime;
using Prime.Common;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    public class OhlcInstantChartPoint : IInstantChartPoint
    {
        public OhlcInstantChartPoint(OhlcEntry i)
        {
            X = i.DateTimeUtc.ToInstant();
            Open = (double)i.Open;
            High = (double)i.High;
            Low = (double)i.Low;
            Close = (double)i.Close;
        }

        public Instant X { get; set; }

        public double Open { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Close { get; set; }

        public override string ToString()
        {
            return $"{X} O:{Open} H:{High} L:{Low} C:{Close}";
        }
    }
}