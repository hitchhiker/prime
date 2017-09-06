using System;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class LogPanelItemViewModel
    {
        public LoggingLevel LogLevel { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public string Message { get; set; }
    }
}