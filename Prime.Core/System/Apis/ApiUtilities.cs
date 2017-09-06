using System;
using System.Collections.Concurrent;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class ApiUtilities
    {
        private ApiUtilities() {}

        public static ApiUtilities I => Lazy.Value;
        private static readonly Lazy<ApiUtilities> Lazy = new Lazy<ApiUtilities>(()=>new ApiUtilities());

        /// <summary>
        /// Gets a supposedly unique incrementing (long) value, based on the time + seconds + ms.
        /// </summary>
        /// <returns></returns>
        public long AscendingLongNext()
        {
            var currentTime = DateTime.Now;
            var dt = currentTime.ToUniversalTime();
            var myEpoch = new DateTime(2017, 7, 11);
            var s = ((long)dt.Subtract(myEpoch).TotalSeconds).ToString();
            return (s + currentTime.Millisecond.ToString("D4")).ToLong();
        }
    }
}