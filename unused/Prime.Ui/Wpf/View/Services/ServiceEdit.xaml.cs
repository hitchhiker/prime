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
using MahApps.Metro.Controls.Dialogs;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for ServiceEdit.xaml
    /// </summary>
    public partial class ServiceEdit : UserControl
    {
        // Here we create the viewmodel with the current DialogCoordinator instance 

        readonly DialogueVm _vm = new DialogueVm(DialogCoordinator.Instance);

        public ServiceEdit()
        {
            InitializeComponent();
            DataContext = _vm;
            this.Loaded += (o, args) => Keyboard.Focus(ApiName);
        }
    }
}
