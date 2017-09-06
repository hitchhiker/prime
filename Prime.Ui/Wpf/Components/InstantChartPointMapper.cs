using System;
using Prime.Core;

namespace Prime.Ui.Wpf
{
    public class InstantChartPointMapper : CartesianMapperBase<InstantChartPoint>
    {
        public InstantChartPointMapper(IResolutionSource source) : base(source)
        {
            X(m =>
            {
                switch (source.Resolution)
                {
                    case TimeResolution.Millisecond:
                        return m.X.ToUnixTimeTicks();

                    case TimeResolution.Second:
                        return m.X.ToUnixTimeSeconds();

                    case TimeResolution.Minute:
                        return m.X.ToUnixTimeSeconds() / 60;

                    case TimeResolution.Hour:
                        return m.X.ToUnixTimeSeconds() / 60 / 60;

                    case TimeResolution.Day:
                        return m.X.ToUnixTimeSeconds() / 60 / 60 / 24;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            Y(m => (double)m.Y);
        }
    }
}