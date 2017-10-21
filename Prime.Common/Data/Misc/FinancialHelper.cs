using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common
{
    public class FinancialHelper
    {
        public static IList<double> ComputeMovingAverage(IList<double> values, int length)
        {
            var numArray = new double[values.Count];
            for (var to = 0; to < values.Count; ++to)
                numArray[to] = to >= length ? AverageOf(values, to - length, to) : double.NaN;
            return numArray;
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
