using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class LogPanelViewModel : ToolPaneViewModel
    {
        public LogPanelViewModel()
        {
            Key = null;
            M.RegisterAsync<NewLogMessage>(this, Key, AddEntry);
            base.Title = "Event log";
        }

        private void Clear()
        {
            LogEntries.Clear();
            Count = LogEntries.Count;
        }
        
        public BindingList<LogEntry> LogEntries { get; private set; } = new BindingList<LogEntry>();

        private int _count;
        public int Count
        {
            get => _count;
            set => SetAfter(ref _count, value, c=> Title = ("Event Log (" + c + ")"));
        }
        
        private readonly object _lock = new object();

        private void AddEntry(NewLogMessage message)
        {
            UiDispatcher.Invoke(() =>
            {
                lock (_lock)
                {
                    if (message.SameLine && LogEntries.Count > 0)
                        LogEntries.RemoveAt(LogEntries.Count - 1);

                    LogEntries.Add(new LogEntry
                    {
                        DateTime = message.UtcCreated.ToLocalTime(),
                        Index = LogEntries.Count,
                        Message = message.Message
                    });

                    Count = LogEntries.Count;
                }
            });
        }

        public override void Dispose()
        {
            M.UnregisterAsync(this);
            base.Dispose();
        }
    }
}