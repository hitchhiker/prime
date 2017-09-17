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
        private readonly StatusViewModel _statusViewModel;

        public MainWindow()
        {
            InitializeComponent();

            Title = "prime radiant [alpha " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "] - decentralised bootstrapper [ipfs]";

            WindowState = WindowState.Minimized;
        
            var dispatcher = Dispatcher;

            Action<string> nativeLogger = (Terminal.DataContext as TerminalViewModel).AddItem;

            var logger = Logging.I.DefaultLogger;

            DefaultMessenger.I.Default.Register<NewLogMessage>(this, m =>
            {
                dispatcher.Invoke(() => nativeLogger.Invoke(LanguageCorrection(m.Message)));
            });

            var manager = new DeploymentManager(dispatcher, logger, () => { dispatcher.Invoke(() => WindowState = WindowState.Normal); });
            
            DataContext = _statusViewModel = new StatusViewModel(manager);

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

        private void I_OnNewMessage(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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