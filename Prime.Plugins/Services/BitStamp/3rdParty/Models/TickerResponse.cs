using System;

namespace Rokolab.BitstampClient.Models
{
    public class TickerResponse
    {
        public string last { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string vwap { get; set; }
        public string volume { get; set; }
        public string bid { get; set; }
        public string ask { get; set; }

        public string avg
        {
            get
            {
                try
                {
                    return ((Convert.ToDouble(high.Replace(".", ","))
                        + Convert.ToDouble(low.Replace(".", ","))) / 2).ToString("f4").Replace(",", ".");
                }
                catch
                {
                    return "0.0000";
                }
            }
        }
    }
}