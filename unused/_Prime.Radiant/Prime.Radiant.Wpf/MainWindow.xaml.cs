#region

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Prime.Utility;

#endregion

namespace Prime.Radiant
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;

            Title = "prime radiant [alpha " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "] - decentralised bootstrapper [ipfs]";

            WindowState = WindowState.Minimized;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var dispatcher = Dispatcher;

            Action<string> nativeLogger = (Terminal.DataContext as TerminalViewModel).AddItem;

            var logger = Logging.I.DefaultLogger;

            DefaultMessenger.I.Default.Register<NewLogMessage>(this, m =>
            {
                dispatcher.Invoke(() => nativeLogger.Invoke(LanguageCorrection(m.Message)));
            });

            var manager = new DeploymentManager(dispatcher, logger, () => { dispatcher.Invoke(() => WindowState = WindowState.Normal); });

            DataContext = new StatusViewModel(manager);

            var dom = AppDomain.CurrentDomain;

            dom.UnhandledException +=
                (o, args) => dispatcher.Invoke(() => nativeLogger.Invoke("Fatal (dom): " + (args.ExceptionObject as Exception)?.Message));

            Application.Current.DispatcherUnhandledException +=
                (o, args) => dispatcher.Invoke(() => nativeLogger.Invoke("Fatal (app): " + args.Exception.Message));

            ThreadPool.QueueUserWorkItem(w =>
            {
                manager.Bootstrap();
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