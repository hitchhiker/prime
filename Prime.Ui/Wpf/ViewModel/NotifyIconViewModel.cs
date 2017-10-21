using System.Windows;
using System.Windows.Input;
using Prime.Common;

namespace Prime.Ui.Wpf.ViewModel
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        private readonly IWindowManager _manager = UserContext.Current.WindowManager;

        public ICommand DoubleClickWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => true,
                    CommandAction = () => _manager.Toggle()
                };
            }
        }

        public ICommand NewWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => _manager.CanSpawn(),
                    CommandAction = () => _manager.CreateNewWindow()
                };
            }
        }

        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowAllWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => _manager.CanShowAll(),
                    CommandAction = () => _manager.ShowAll()
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand MinimiseAllWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => _manager.CanMinimiseAll(),
                    CommandAction = () => _manager.MinimiseAll()
                };
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand {CommandAction = () => Application.Current.Shutdown()};
            }
        }
    }
}
