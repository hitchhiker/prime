using System;
using NodaTime;
using Prime.Core;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    public struct InstantChartPoint : IInstantChartPoint
    {
        public Instant X { get; set; }

        public decimal Y { get; set; }

        public InstantChartPoint(OhlcEntry i)
        {
            X = i.DateTimeUtc.ToInstant();
            Y = (decimal) i.Close;
        }

        public InstantChartPoint(DateTime dateTimeUtc, decimal y)
        {
            X = dateTimeUtc.ToInstant();
            Y = y;
        }

        public InstantChartPoint(Instant x, decimal y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X} - {Y}";
        }
    }
}