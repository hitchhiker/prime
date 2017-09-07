using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Prime.Core;
using Framework.UI;
using Hardcodet.Wpf.TaskbarNotification;
using Prime.Utility;
using Prime.Ui.Wpf;
using Prime.Ui.Wpf.ViewModel;

namespace prime
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private TaskbarIcon notifyIcon;

        public App()
        {
            //DispatcherUnhandledException += App_DispatcherUnhandledException;
            GlobalMisc.I.MainAssembly = Assembly.GetExecutingAssembly();

            this.Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            UserContext.Current.WindowManager.Init();
        }
        
        /*
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var exceptionView = new ExceptionView(e.Exception)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            exceptionView.ShowDialog();

            e.Handled = true;
        }*/

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            Logging.I.OnNewMessage += I_OnNewMessage;
        }

        private void I_OnNewMessage(object sender, EventArgs e)
        {
            var me = e as LoggerMessageEvent;
            if (me == null)
                return;

            PrimeWpf.I.Messenger.Send(new LogEntryReceivedMessage(me));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}
