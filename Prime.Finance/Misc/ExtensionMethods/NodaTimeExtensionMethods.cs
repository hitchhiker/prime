using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace Prime.Core
{
    public static class NodaTimeExtensionMethods
    {
        public static Instant ToInstantLocal(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException(nameof(dateTime) + " must be of Kind " + DateTimeKind.Utc);

            var utcf = new DateTime(dateTime.ToLocalTime().Ticks, DateTimeKind.Utc);

            return Instant.FromDateTimeUtc(utcf);
        }

        public static Instant ToInstant(this DateTime dateTime)
        {
            return Instant.FromDateTimeUtc(dateTime);
        }
    }
}
