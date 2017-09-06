using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prime.Core;
using Ookii.Dialogs.Wpf;

namespace prime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = "prime [alpha " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "] - Supranational asset manager."; 
        }
    }
}
