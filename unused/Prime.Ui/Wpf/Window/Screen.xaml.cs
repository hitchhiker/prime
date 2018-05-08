using System;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Prime.Ui.Wpf;
using MahApps.Metro.Controls;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Screen : MetroWindow
    {
        private readonly IMessenger _messenger;

        public ScreenViewModel ViewModel => (ScreenViewModel)DataContext;

        public Screen()
        {            
            InitializeComponent();

            // TODO: HH: Implement dependency injection for windows.
            _messenger = Messenger.Default; 
        }
    }
}
