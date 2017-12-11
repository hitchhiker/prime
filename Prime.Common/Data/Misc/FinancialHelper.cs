using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common
{
    public static class FinancialHelper
    {
        public static IList<double> ComputeMovingAverage(this IList<double> values, int length)
        {
            var numArray = new double[values.Count];
            for (var to = 0; to < values.Count; ++to)
                numArray[to] = to >= length ? AverageOf(values, to - length, to) : double.NaN;
            return numArray;
        }

        public static IList<decimal> ComputeMovingAverage(this IList<decimal> values, int period, int length)
        {
            var buffer = new decimal[period];
            var output = new decimal[values.Count];
            var index = 0;
            for (var i = 0; i < values.Count; i++)
            {
                buffer[index] = values[i] / period;
                var ma = 0.0m;
                for (var j = 0; j < period; j++)
                    ma += buffer[j];
                output[i] = ma;
                index = (index + 1) % period;
            }
            return output;
        }

        public static List<decimal> SimulatePeriod(this IList<Tuple<DateTime, decimal>> values, TimeSpan period)
        {
            var r = new List<decimal>();

            if (!values.Any())
                return r;

            r.Add(values[0].Item2);

            if (values.Count == 1)
                return r;

            var sort = values.OrderBy(x => x.Item1).ToList();

            var lowDate = values.FirstOrDefault().Item1;
            var highDate = values.LastOrDefault().Item1;

            for (var i = lowDate.Add(period); i <= highDate.Add(period); i = i.Add(period))
            {
                var eq = sort.FirstOrDefault(x => x.Item1 == i);
                if (eq!=null)
                {
                    r.Add(eq.Item2);
                    continue;
                }

                var low = sort.LastOrDefault(x => x.Item1 < i);
                var high = sort.FirstOrDefault(x => x.Item1 > i);

                if (high == null)
                {
                    r.Add(low.Item2);
                    continue;
                }

                var distance = (high.Item1 - low.Item1).Ticks;
                var tickDistance = (i - low.Item1).Ticks;
                var tickPerc = (1m / distance) * tickDistance;
                var valDiff = (high.Item2 - low.Item2) * tickPerc;
                r.Add(low.Item2 + valDiff);
            }

            return r;
        }

        public static double AverageOf(IList<double> values, int from, int to)
        {
            var num = 0.0;
            for (var index = from; index < to; ++index)
                num += values[index];
            return num / (to - from);
        }
    }
}
