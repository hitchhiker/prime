using System;
using Prime.Common;

namespace Prime.Ui.Wpf
{
    public class OhlcInstantChartPointMapper : FinancialMapperBase<OhlcInstantChartPoint>
    {
        public OhlcInstantChartPointMapper(IResolutionSource source) : base(source)
        {
            X(m => PointMapperHelpers.ToX(source, m.X));
            Open(m => m.Open);
            Close(m => m.Close);
            High(m => m.High);
            Low(m => m.Low);
        }
    }
}