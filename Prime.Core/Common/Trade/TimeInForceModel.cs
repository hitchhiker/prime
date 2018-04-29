using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Trade
{
    public class TimeInForceModel
    {
        public int Key { get; }
        public string TimeInForceName { get; }

        public TimeInForceModel(int key, string timeInForceName)
        {
            this.Key = key;
            this.TimeInForceName = timeInForceName;
        }
    }
}
