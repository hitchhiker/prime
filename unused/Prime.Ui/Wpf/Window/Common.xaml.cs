using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Prime.Core;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using MahApps.Metro.Controls;
using Ookii.Dialogs.Wpf;
using plugins;
using Prime.Utility;
using Prime.Ui.Wpf.ViewModel;
using Prime.Ui.Wpf.Pages;

namespace prime.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Common : UserControl
    {
        private TextBoxStreamWriter _writer;
        private ModernFrame _innerTarget;

        public Common()
        {
            InitializeComponent();

            DataContextChanged += Common_DataContextChanged;
        }

        private void Common_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = this.DataContext as LayoutViewModel;
            if (vm == null)
                return;

            Console.SetOut(_writer = Cns.Writer);

            _innerTarget = NavigationHelper.FindFrame("ContentFrame", this);

            vm.NavigationProvider.GoAction = delegate (NavigationEndpoint endpoint, UserContext context)
            {
                NavigationCommands.GoToPage.Execute(endpoint.Destination, _innerTarget);
                _innerTarget.DataContext = new LiveChartOhclDesignViewModel();
            };
        }
    }
}
