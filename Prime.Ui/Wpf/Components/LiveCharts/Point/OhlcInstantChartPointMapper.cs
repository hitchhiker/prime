using System;
using Prime.Core;

namespace Prime.Ui.Wpf
{
    public class OhlcInstantChartPointMapper : FinancialMapperBase<OhlcInstantChartPoint>
    {
        public OhlcInstantChartPointMapper(IResolutionSource source) : base(source)
        {
            X(m =>
            {
                switch (source.Resolution)
                {
                    case TimeResolution.Millisecond:
                        return m.X.ToUnixTimeMilliseconds();

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
            Open(m => m.Open);
            Close(m => m.Close);
            High(m => m.High);
            Low(m => m.Low);
        }
    }
}