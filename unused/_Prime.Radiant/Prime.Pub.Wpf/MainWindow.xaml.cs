#region

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Prime.Radiant.Components;
using Prime.Utility;

#endregion

namespace Prime.Radiant
{
    /// <summary>
    /// Don't hate bro ;_ , I had a few days to do this, while hassling with a million other things.
    /// </summary>
    /// 
    public partial class MainWindow
    {
        private readonly StatusViewModel _statusViewModel;
        private readonly PublishManager _manager;
        public MainWindow()
        {
            InitializeComponent();

            Title = "prime pub [alpha " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "] - decentralised bootstrapper [ipfs]";

            var dispatcher = Dispatcher;

            Action<string> nativeLogger = (Terminal.DataContext as TerminalViewModel).AddItem;

            var logger = Logging.I.DefaultLogger;

            DefaultMessenger.I.Default.Register<NewLogMessage>(this, m =>
            {
                dispatcher.Invoke(() => nativeLogger.Invoke(LanguageCorrection(m.Message)));
            });

            var pc = PublishManagerContext.LoadDefault(Dispatcher.CurrentDispatcher);
            if (pc == null)
            {
                logger.Error("Configuration not found.");
                return;
            }

            pc.L = logger;
            var dom = AppDomain.CurrentDomain;

            dom.UnhandledException +=
                (o, args) => dispatcher.Invoke(() => nativeLogger.Invoke("Fatal (app domain): " + (args.ExceptionObject as Exception)?.Message));

            Application.Current.DispatcherUnhandledException +=
                (o, args) => dispatcher.Invoke(() => nativeLogger.Invoke("Fatal (app): " + args.Exception.Message));

            _manager = new PublishManager(pc);

            DataContext = _statusViewModel = new StatusViewModel(_manager);

            Prime.Click += Prime_Click;
            Radiant.Click += Radiant_Click;
            Both.Click += Both_Click;
        }

        private void Both_Click(object sender, RoutedEventArgs e)
        {
            _manager.Context.DoPrime = _manager.Context.DoRadiant = true;
            Start();
        }

        private void Radiant_Click(object sender, RoutedEventArgs e)
        {
            _manager.Context.DoRadiant = true;
            Start();
        }

        private void Prime_Click(object sender, RoutedEventArgs e)
        {
            _manager.Context.DoPrime = true;
            Start();
        }

        private void Start()
        {
            (Terminal.DataContext as TerminalViewModel).Clear();
            Chooser.Visibility = Visibility.Collapsed;
            ThreadPool.QueueUserWorkItem(w =>
            {
                _manager.Start();
                _manager.IpfsDaemon.Stop();
                Dispatcher.Invoke(() =>
                {
                    Chooser.Visibility = Visibility.Visible;
                });
            });
        }

        private string LanguageCorrection(string s)
        {
            if (s.Contains("nitializing"))
                s = s.Replace("nitializing", "nitialising"); // it's all love 'murica ;)

            if (s.Contains("generating"))
                s = s.Replace("generating", "Generating");

            return s;
        }
    }
}