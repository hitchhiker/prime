using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using SharpDX.Collections;

namespace Prime.Ui.Wpf.ViewModel
{
    public class LogPanelViewModel : ToolPaneViewModel
    {
        private readonly IMessenger _messenger;
        private readonly Dispatcher _dispatcher;
        public LogPanelViewModel() { }

        public LogPanelViewModel(IMessenger messenger, string key = null)
        {
            Key = key;
            _dispatcher = Application.Current.Dispatcher;
            _messenger = messenger;
            _messenger.Register<LogEntryReceivedMessage>(this, AddEntry);
            Name = "Event log";
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
            set => SetAfter(ref _count, value, c=> Name = ("Event Log (" + c + ")"));
        }
        
        private readonly object _lock = new object();

        private void AddEntry(LogEntryReceivedMessage message)
        {
            if (Key != null && !Key.Equals(message.Key, StringComparison.OrdinalIgnoreCase))
                return;

            lock (_lock)
            {
                _dispatcher.Invoke(() =>
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
                });
            }
        }
    }
}