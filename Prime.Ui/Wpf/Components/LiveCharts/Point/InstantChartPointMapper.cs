using System;
using NodaTime;
using Prime.Core;

namespace Prime.Ui.Wpf
{
    public class InstantChartPointMapper : CartesianMapperBase<InstantChartPoint>
    {
        public InstantChartPointMapper(IResolutionSource source) : base(source)
        {
            X(m => PointMapperHelpers.ToX(source, m.X));
            Y(m => (double)m.Y);
        }
    }
}