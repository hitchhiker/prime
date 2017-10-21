using System;
using NodaTime;
using Prime.Common;

namespace Prime.Ui.Wpf
{
    public class PointMapperHelpers
    {
        public static double ToX(IResolutionSource source, Instant instant)
        {
            var b = GetSeconds(instant, source.Resolution);

            switch (source.Resolution)
            {
                case TimeResolution.Second:
                    return b;

                case TimeResolution.Minute:
                    return b / 60;

                case TimeResolution.Hour:
                    return b / 60 / 60;

                case TimeResolution.Day:
                    return b / 60 / 60 / 24;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double GetSeconds(Instant i, TimeResolution resolution)
        {
            return i.ToUnixTimeSeconds();
        }
    }
}