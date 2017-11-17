
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prime.Utility
{
    public static class FinanceExtensionMethods
    {
        /// <summary>
        /// Calculates standard deviation
        /// https://stackoverflow.com/a/6252351/1318333
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        public static double ApplyCompoundPercent(this double startingValue, double percentIncrease, double times)
        {
            return Math.Pow(1 + (percentIncrease/100), times) * startingValue;
        }

        public static double GetCompoundPercentChange(this double percentIncrease, double times)
        {
            var nv = ApplyCompoundPercent(1, percentIncrease, times);
            return 100  * (nv - 1);
        }

        public static decimal ApplyCompoundPercent(this decimal startingValue, decimal percentIncrease, double times)
        {
            var pi = (double) percentIncrease;
            return (decimal)Math.Pow(1 + (pi / 100), times) * startingValue;
        }

        public static decimal GetCompoundPercentChange(this decimal percentIncrease, double times)
        {
            var nv = ApplyCompoundPercent(1, percentIncrease, times);
            return 100 * (nv - 1);
        }
    }
}
