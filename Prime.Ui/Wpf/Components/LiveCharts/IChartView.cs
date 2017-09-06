using System.Collections.Generic;
using NodaTime;

namespace Prime.Ui.Wpf
{
    public interface IChartView : IResolutionSource, IChartParser
    {
        /// <summary>
        /// Gets the dictionary containing the Last Update instant for a specific series
        /// </summary>
        Dictionary<string, Instant> LastUpdates { get; }

        /// <summary>
        /// Gets or sets whether the view position of the chart is locked
        /// </summary>
        /// <remarks>When unlocked, the chart wil automatically scroll to new updates</remarks>
        bool IsPositionLocked { get; set; }
    }
}