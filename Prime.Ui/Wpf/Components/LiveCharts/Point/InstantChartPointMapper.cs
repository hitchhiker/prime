using System;
using NodaTime;
using Prime.Core;

namespace Prime.Ui.Wpf
{
    public class InstantChartPointMapper : CartesianMapperBase<InstantChartPoint>
    {
        public InstantChartPointMapper(IResolutionSource source) : base(source)
        {
            X(m => ToX(source, m));
            Y(m => (double)m.Y);
        }

        private static double ToX(IResolutionSource source, InstantChartPoint m)
        {
            var b = GetSeconds(m.X, source.Resolution);

            switch (source.Resolution)
            {
                case TimeResolution.Second:
                    return b;

                case TimeResolution.Minute:
                    return b / 60;

                case TimeResolution.Hour:
                    return b / 60 / 60;

                case TimeResolution.Day:
                    return b / 60 / 60 / 24;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double GetSeconds(Instant i, TimeResolution resolution)
        {
            return i.ToUnixTimeSeconds();
        }
    }
}