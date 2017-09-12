using System;
using System.Windows;
using LiveCharts;
using LiveCharts.Charts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Dtos;
using LiveCharts.Helpers;
using NodaTime.Extensions;

namespace Prime.Ui.Wpf
{
    public class PrimeDateAxisCore : DateAxisCore
    {
        private DateTime _initialDateTime = new NodaTime.Instant().ToDateTimeUtc();
        private int _utcOffset = (int) (DateTime.Now - DateTime.UtcNow).TotalHours;

        private PeriodUnit _period => ((IDateAxisView)this.View).Period;

        public PrimeDateAxisCore(IWindowAxisView view) : base(view) {}

        public override Func<double, string> GetFormatter()
        {
            return new Func<double, string>(this.FormatLabel);
        }

        private string FormatLabel(double x)
        {
            DateTime dateTime = this.GetdateTime(x);
            switch (_period)
            {
                case PeriodUnit.Milliseconds:
                    return dateTime.ToString("G") + dateTime.ToString(".fff");
                case PeriodUnit.Seconds:
                    return dateTime.ToString("G");
                case PeriodUnit.Minutes:
                    return dateTime.ToString("g");
                case PeriodUnit.Hours:
                    return dateTime.ToString("g");
                case PeriodUnit.Days:
                    return dateTime.ToString("d");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal DateTime GetdateTime(double x)
        {
            DateTime dateTime;
            switch (_period)
            {
                case PeriodUnit.Milliseconds:
                    dateTime = this._initialDateTime.AddMilliseconds(Math.Floor(x)).AddHours(_utcOffset);
                    break;
                case PeriodUnit.Seconds:
                    dateTime = this._initialDateTime.AddSeconds(Math.Floor(x)).AddHours(_utcOffset);
                    break;
                case PeriodUnit.Minutes:
                    dateTime = this._initialDateTime.AddMinutes(Math.Floor(x)).AddHours(_utcOffset);
                    break;
                case PeriodUnit.Hours:
                    dateTime = this._initialDateTime.AddHours(Math.Floor(x)).AddHours(_utcOffset);
                    break;
                case PeriodUnit.Days:
                    dateTime = this._initialDateTime.AddDays(Math.Floor(x));
                    break;
                default:
                    throw new ArgumentException();
            }

            return dateTime;
        }
    }
}