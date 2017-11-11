
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
    }
}
