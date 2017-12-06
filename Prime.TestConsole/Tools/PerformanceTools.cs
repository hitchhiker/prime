using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.TestConsole.Tools
{
    public class PerformanceTools
    {
        private Stopwatch _stopwatch = new Stopwatch();

        private class TestTicker
        {
            public float vol;
            public decimal last;
            public decimal sell;
            public decimal buy;
            public decimal weekRiseRate;
            public decimal riseRate;
            public decimal high;
            public decimal low;
            public decimal monthRiseRate;
        }

        public void MemTest()
        {
            var r1 = TestSum();
            var r2 = TestBool();
        }

        public void CheckConditionals()
        {
            var n = 10000000;

            _stopwatch.Start();
            for (int i = 0; i < n; i++)
            {
                var r1 = TestSum();
            }
            _stopwatch.Stop();
            Console.WriteLine($"First: {_stopwatch.ElapsedMilliseconds}");

            _stopwatch.Start();
            for (int i = 0; i < n; i++)
            {
                var r2 = TestBool();
            }
            _stopwatch.Stop();
            Console.WriteLine($"Second: {_stopwatch.ElapsedMilliseconds}");
        }

        private bool TestSum()
        {
            var t1 = new TestTicker();

            return (decimal)t1.vol + t1.last + t1.sell + t1.buy + t1.weekRiseRate + t1.riseRate + t1.high + t1.low +
                   t1.monthRiseRate == 0;
        }

        private bool TestBool()
        {
            var t2 = new TestTicker();

            return (decimal)t2.vol == 0 && t2.last == 0 && t2.sell == 0 && t2.buy == 0 && t2.weekRiseRate == 0 && t2.riseRate == 0 && t2.high == 0 && t2.low == 0 && t2.monthRiseRate == 0;
        }
    }
}
