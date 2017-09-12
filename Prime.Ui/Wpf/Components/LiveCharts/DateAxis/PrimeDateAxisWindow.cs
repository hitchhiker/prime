using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;

namespace Prime.Ui.Wpf
{
    public abstract class PrimeDateAxisWindow : DateAxisWindow
    {
        public PrimeDateAxisCore Dac => DateAxisCore as PrimeDateAxisCore;

        public override bool IsHeader(double x)
        {
            return this.IsHeader(this.Dac.GetdateTime(x));
        }

        public override bool IsSeparator(double x)
        {
            return this.IsSeparator(this.Dac.GetdateTime(x));
        }

        public override string FormatAxisLabel(double x)
        {
            return this.FormatAxisLabel(this.Dac.GetdateTime(x));
        }

        public override bool TryGetSeparatorIndices(IEnumerable<double> indices, int maximumSeparatorCount, out IEnumerable<double> separators)
        {
            List<DateTime> list = indices.Take<double>(2).Select<double, DateTime>((Func<double, DateTime>)(d => this.Dac.GetdateTime(d))).ToList<DateTime>();
            if (this.Validate(list[1].Subtract(list[0])))
                return base.TryGetSeparatorIndices(indices, maximumSeparatorCount, out separators);
            separators = Enumerable.Empty<double>();
            return false;
        }
    }
}